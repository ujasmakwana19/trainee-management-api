using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace TraineeManagement.Messaging;

public class RabbitMqEventPublisher : IEventPublisher
{
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMqEventPublisher> _logger;

    public RabbitMqEventPublisher(IConnection connection, ILogger<RabbitMqEventPublisher> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T message, string coorealtionId, string routingKey, CancellationToken cancellationToken = default)
        where T : class
    {
        if (_connection == null || !_connection.IsOpen)
        {
            _logger.LogWarning("RabbitMQ unavailable event not published: {RoutingKey}", routingKey);
            return; // Cause soft dependency
        }

        try
        {
            // This creates the scoped channel it will be , closed as the message is published successfully
            using IChannel channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            // the data (message) must be converted to and fro from json to bytes
            // before sending and after receiving 
            byte[] body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            BasicProperties properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json"
            };
            
            await channel.BasicPublishAsync(
                exchange: QueueConfig.SubmissionExchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken
            );
            _logger.LogInformation("CoorelationID:{CoorelationId} - Event published successfully: QueueBinding:{RoutingKey}", coorealtionId, routingKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event: {RoutingKey}", routingKey);
            return; // Cause soft dependency
        }
    }
}