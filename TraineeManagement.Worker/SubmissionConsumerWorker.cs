using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.TrackTaskModel;
using TraineeManagement.Contracts.Events;
using TraineeManagement.Messaging;
using TraineeManagement.Api.CacheServices;
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
        // Creates the persistant channel connection 
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: cancellationToken);

        _logger.LogInformation("SubmissionConsumerWorker channel ready, listening on queue: {Queue}", RabbitMqSetup.QueueName);

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel!);

        // lambda function that acts as a callback. Every single time RabbitMQ pushes a message from the queue this block of code executes. The ea parameter (Event Arguments) contains the message data and headers.
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
                    // Negative ack
                    await _channel!.BasicNackAsync(
                        ea.DeliveryTag, 
                        multiple: false, 
                        // to move to dead state and remove
                        requeue: false, 
                        cancellationToken: stoppingToken
                    );
                    return;
                }

                messageId = message.MessageId;
                _logger.LogInformation(
                    "Processing submission event | MessageId: {MessageId} | CorrelationId: {CorrelationId} | TaskAssignmentId: {TaskAssignmentId}",
                    message.MessageId, message.CorrelationId, message.TaskAssignmentId
                );

                await ProcessMessageAsync(message, stoppingToken);
                
                // Positive Ack
                await _channel!.BasicAckAsync(
                    ea.DeliveryTag, 
                    multiple: false, 
                    cancellationToken: stoppingToken
                );
                _logger.LogInformation("Acked message {MessageId}", messageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message {MessageId}. Requeuing.", messageId ?? "unknown");

                // requeue: true put it back in the queue for retry
                await _channel!.BasicNackAsync(
                    ea.DeliveryTag, 
                    multiple: false, 
                    requeue: true, 
                    cancellationToken: stoppingToken
                );
            }
        };

        // This is the method that use the callback
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
        // just added for monitoring
        await Task.Delay(TimeSpan.FromSeconds(20), cancellationToken);
        
        // scope for the DB operation
        using (IServiceScope scope = _scopeFactory.CreateScope())
        {
            
            AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            ICacheService cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

            TrackTask? task = await context.TrackTasks.FirstOrDefaultAsync(t => t.Id == message.TaskAssignmentId, cancellationToken);

            if (task is null)
            {
                _logger.LogWarning("TrackTask with Id {Id} not found", message.TaskAssignmentId);
                return;
            }

            task.Status = TaskAssignmentValue.Submitted;
            await context.SaveChangesAsync(cancellationToken);
            await cacheService.RemoveAsync(CacheKey.trackTaskId + $"{task.Id}");
            await cacheService.RemoveAsync(CacheKey.trackTaskAll);
            _logger.LogInformation("TrackTask {Id} marked as Submitted", message.TaskAssignmentId);
        }
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