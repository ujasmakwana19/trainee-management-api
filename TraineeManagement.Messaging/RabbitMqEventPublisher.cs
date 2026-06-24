using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace TraineeManagement.Messaging;

public class RabbitMqEventPublisher : IEventPublisher
{
    private readonly IConnection _connection;

    public RabbitMqEventPublisher(IConnection connection)
    {
        _connection = connection;
    }

    public async Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default)
        where T : class
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
            exchange: RabbitMqSetup.ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken
        );
    }
}