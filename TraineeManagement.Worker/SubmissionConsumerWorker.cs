using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.TrackTaskModel;
using TraineeManagement.Api.CacheServices;
using TraineeManagement.Data.ProcessingJobModel;
using TraineeManagement.Messaging;
using TraineeManagement.Api.SubmissionFileModel;

namespace TraineeManagement.Worker;

public class SubmissionConsumerWorker : BackgroundService
{
    private readonly IConnection _connection;
    private readonly ILogger<SubmissionConsumerWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _localStoragePath;
    private IChannel? _channel;

    private const int MaxRetryAttempts = 3;

    public SubmissionConsumerWorker(
        IConnection connection,
        ILogger<SubmissionConsumerWorker> logger,
        IServiceScopeFactory scopeFactory,
        IConfiguration config)
    {
        _connection = connection;
        _logger = logger;
        _scopeFactory = scopeFactory;
        _localStoragePath = config["StorageSettings:StorageRoot"]!;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        // Fetch one message at a time to distribute loads evenly across workers
        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: cancellationToken);

        _logger.LogInformation("SubmissionConsumerWorker channel ready, listening on queue: {Queue}", QueueConfig.SubmissionQueue);

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel!);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            ProcessingJob? jobContext = null;

            try
            {
                string json = Encoding.UTF8.GetString(ea.Body.ToArray());
                ProcessingJob? incomingMessage = JsonSerializer.Deserialize<ProcessingJob>(json);

                if (incomingMessage is null || incomingMessage.MessageId == Guid.Empty)
                {
                    _logger.LogWarning("Received null, corrupt, or un-deserializable message. Dead-lettering immediately.");
                    await RejectMessageAsync(
                        ea.DeliveryTag,
                        requeue: false,
                        stoppingToken
                    );
                    return;
                }


                jobContext = await isMessageAvailableToBeProcessed(incomingMessage);

                if (jobContext is null)
                {
                    _logger.LogInformation(
                        "Duplicate or already processed message ignored MessageId: {MessageId} Submission: {SubmissionId}",
                        incomingMessage.MessageId, incomingMessage.SubmissionId
                    );

                    await AckMessageAsync(ea.DeliveryTag, stoppingToken);
                    return;
                }

                // Business logic to be performed
                await ProcessMessageInternalAsync(jobContext, stoppingToken);

                await UpdateJobStatusAsync(
                    jobContext.MessageId,
                    ProcessingJobStatus.Completed,
                    null,
                    stoppingToken
                );

                await AckMessageAsync(ea.DeliveryTag, stoppingToken);
                _logger.LogInformation("Successfully processed and Acked message {MessageId}", jobContext.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception caught while processing message delivery tag: {DeliveryTag}", ea.DeliveryTag);

                if (jobContext is null)
                {
                    // If not parse or grab a Job record context, safely reject it to Dead Letter Queue
                    await RejectMessageAsync(ea.DeliveryTag, requeue: false, stoppingToken);
                    return;
                }
                await HandleProcessingFailureAsync(ea.DeliveryTag, jobContext, ex, stoppingToken);
            }
        };

        await _channel!.BasicConsumeAsync(
            queue: QueueConfig.SubmissionQueue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }


    private async Task<ProcessingJob?> isMessageAvailableToBeProcessed(ProcessingJob incomingMessage)
    {
        using (IServiceScope scope = _scopeFactory.CreateScope())
        {

            AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            ProcessingJob? existingJob = await context.ProcessingJobs
                .FirstOrDefaultAsync(t => t.MessageId == incomingMessage.MessageId);

            if (existingJob is null || existingJob.Status == ProcessingJobStatus.Completed || existingJob.Status == ProcessingJobStatus.Processing)
            {
                return null;
            }

            existingJob.Status = ProcessingJobStatus.Processing;
            existingJob.StartedAt = DateTime.UtcNow;
            existingJob.Attempts += 1;

            await context.SaveChangesAsync();
            return existingJob;
        }
    }


    private async Task ProcessMessageInternalAsync(ProcessingJob message, CancellationToken cancellationToken)
    {

        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

        // To test the dead queue part
        // throw new Exception("Hello");
        using (IServiceScope scope = _scopeFactory.CreateScope())
        {

            AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            ICacheService cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

            SubmissionFile? file = await context.SubmissionFiles.FirstOrDefaultAsync(t => t.Id == message.SubmissionId, cancellationToken);
            if (file is not null && !string.IsNullOrEmpty(file.Checksum))
            {
                IEnumerable<SubmissionFile?> existingSameFiles = await context.SubmissionFiles.Where(
                    t => t.Checksum == file.Checksum &&
                    t.Id != file.Id).ToListAsync();

                foreach (SubmissionFile? existingSameFile in existingSameFiles)
                {
                    if (existingSameFile is not null && !string.IsNullOrEmpty(existingSameFile.Checksum))
                    {
                        if (File.Exists(Path.Combine(_localStoragePath, existingSameFile.StorageName)))
                        {
                            string filePath = Path.Combine(_localStoragePath, file.StorageName);
                            file.StorageName = existingSameFile.StorageName;

                            await context.SaveChangesAsync();

                            if (File.Exists(filePath))
                            {
                                _logger.LogInformation("Completed Processing and Replace the filled with already existed same file");
                                File.Delete(filePath);
                            }
                            return;
                        }
                    }
                }
            }
        }
    }

    private async Task HandleProcessingFailureAsync(ulong deliveryTag, ProcessingJob jobContext, Exception exception, CancellationToken token)
    {
        bool isPermanentError = IsPermanentException(exception);

        if (isPermanentError || jobContext.Attempts >= MaxRetryAttempts)
        {
            _logger.LogCritical("Permanent error or Max attempts reached for Message {MessageId}. Dead-lettering.", jobContext.MessageId);

            string errorSummary = $"[{exception.GetType().Name}]: {exception.Message}";
            await UpdateJobStatusAsync(jobContext.MessageId, ProcessingJobStatus.Failed, errorSummary, token);

            await RejectMessageAsync(deliveryTag, requeue: false, token);
        }
        else
        {
            _logger.LogWarning("Processing failure encountered for Message {MessageId}. Re-queuing message.",
                jobContext.MessageId);
            await Task.Delay(TimeSpan.FromSeconds(10));
            await UpdateJobStatusAsync(jobContext.MessageId, ProcessingJobStatus.Queued, exception.Message, token);
            // Requeue 
            await Task.Delay(TimeSpan.FromSeconds(10));
            await RejectMessageAsync(deliveryTag, requeue: true, token);
        }
    }

    private bool IsPermanentException(Exception ex)
    {
        // Classify errors that retrying will never fix e.g., Parsing errors, bad user parameters, null schemas
        return ex is JsonException || ex is ArgumentException || ex is NullReferenceException;
    }

    private async Task UpdateJobStatusAsync(Guid messageId, ProcessingJobStatus status, string? errorSummary, CancellationToken token)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        ProcessingJob? job = await context.ProcessingJobs.FirstOrDefaultAsync(j => j.MessageId == messageId, token);
        if (job is not null)
        {
            job.Status = status;
            job.ErrorSummary = errorSummary;
            if (status == ProcessingJobStatus.Completed || status == ProcessingJobStatus.Failed)
            {
                job.CompletedAt = DateTime.UtcNow;
            }
            await context.SaveChangesAsync(token);
        }
    }

    private async Task AckMessageAsync(ulong deliveryTag, CancellationToken token)
    {
        if (_channel is not null)
        {
            await _channel.BasicAckAsync(
                deliveryTag,
                multiple: false,
                cancellationToken: token
            );
        }
    }

    private async Task RejectMessageAsync(ulong deliveryTag, bool requeue, CancellationToken token)
    {
        if (_channel is not null)
        {
            await _channel.BasicNackAsync(
                    deliveryTag,
                    multiple: false,
                    requeue: requeue,
                    cancellationToken: token
                );
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("SubmissionConsumerWorker stopping, closing channel cleanly.");
        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken);
            _channel.Dispose();
        }
        await base.StopAsync(cancellationToken);
    }
}