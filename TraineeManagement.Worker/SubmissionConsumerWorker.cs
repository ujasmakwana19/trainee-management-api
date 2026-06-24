using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.TrackTaskModel;
using TraineeManagement.Contracts.Events;
using TraineeManagement.Messaging;

namespace TraineeManagement.Worker;

public class SubmissionConsumerWorker : BackgroundService
{
    private readonly IConnection _connection;
    private readonly ILogger<SubmissionConsumerWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;  
    private IChannel? _channel;

    public SubmissionConsumerWorker(
        IConnection connection,
        ILogger<SubmissionConsumerWorker> logger,
        IServiceScopeFactory scopeFactory)  
    {
        _connection = connection;
        _logger = logger;
        _scopeFactory = scopeFactory;  
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: cancellationToken);

        _logger.LogInformation("SubmissionConsumerWorker channel ready, listening on queue: {Queue}", RabbitMqSetup.QueueName);

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel!);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            string? messageId = null;
            try
            {
                string json = Encoding.UTF8.GetString(ea.Body.ToArray());
                SubmissionProcessingRequested? message = JsonSerializer.Deserialize<SubmissionProcessingRequested>(json);

                if (message is null)
                {
                    _logger.LogWarning("Received null/undeserializable message. Dead-lettering.");
                    await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                    return;
                }

                messageId = message.MessageId;
                _logger.LogInformation(
                    "Processing submission event | MessageId: {MessageId} | CorrelationId: {CorrelationId} | TaskAssignmentId: {TaskAssignmentId}",
                    message.MessageId, message.CorrelationId, message.TaskAssignmentId
                );

                await ProcessMessageAsync(message, stoppingToken);

                await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                _logger.LogInformation("Acked message {MessageId}", messageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message {MessageId}. Requeuing.", messageId ?? "unknown");

                // requeue: true — put it back in the queue for retry
                // In production you'd want a retry count check + dead-letter queue instead
                await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
            }
        };

        await _channel!.BasicConsumeAsync(
            queue: RabbitMqSetup.QueueName,
            autoAck: false,        
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ProcessMessageAsync(SubmissionProcessingRequested message, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(20), cancellationToken);
        using IServiceScope scope = _scopeFactory.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        TrackTask? task = await context.TrackTasks.FirstOrDefaultAsync(t => t.Id == message.TaskAssignmentId, cancellationToken);

        if (task is null)
        {
            _logger.LogWarning("TrackTask with Id {Id} not found", message.TaskAssignmentId);
            return;
        }

        task.Status = TaskAssignmentValue.Submitted;
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("TrackTask {Id} marked as Submitted", message.TaskAssignmentId);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("SubmissionConsumerWorker stopping, closing channel.");
        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken);
            _channel.Dispose();
        }
        await base.StopAsync(cancellationToken);
    }
}