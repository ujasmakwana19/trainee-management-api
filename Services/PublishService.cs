using System.Text.Json;
using RabbitMQ.Client;
using Rabbit.Contracts;

public class PublishService
{
    private readonly IConnection _connection;

    public PublishService(IConnection connection)
    {
        _connection = connection;
    }

    public async Task PublishSubmissionAsync(SubmissionProcessingRequested message)
    {
        using IChannel channel = await _connection.CreateChannelAsync();

        byte[] body = JsonSerializer.SerializeToUtf8Bytes(message);
        BasicProperties properties = new BasicProperties { 
            DeliveryMode = DeliveryModes.Persistent 
        };

        await channel.BasicPublishAsync(
            exchange: "submissions.exchange",
            routingKey: "submission.requested",
            mandatory: true,
            basicProperties: properties,
            body: body
        );
    }
}