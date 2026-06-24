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

    public async Task PublishAsync<T>(T message, string routingKey = RabbitMqSetup.RoutingKey, CancellationToken cancellationToken = default)
        where T : class
    {
        using IChannel channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

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