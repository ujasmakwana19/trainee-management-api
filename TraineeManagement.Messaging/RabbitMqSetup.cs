using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace TraineeManagement.Messaging;

public static class RabbitMqSetup
{
    public const string ExchangeName = "submissions.exchange";
    public const string QueueName = "submission-processing";
    public const string RoutingKey = "submission.requested";

    // For connecting to the RabbitMQ
    public static ConnectionFactory GetConnectionFactory(IConfiguration configuration)
    {
        IConfiguration section = configuration.GetSection("RabbitMQ");
        int.TryParse(section["Port"], out int port);

        return new ConnectionFactory
        {
            HostName = section["HostName"]!,
            Port = port,
            UserName = section["UserName"]!,
            Password = section["Password"]!,
            VirtualHost = section["VirtualHost"] ?? "/",
            // It is used to handle the connection automatically if 
            // if MQ does not exists or the failed to connect via starting up
            AutomaticRecoveryEnabled = true
        };
    }

    public static async Task InitializeTopologyAsync(IConnection connection)
    {
        using IChannel channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Direct,
            durable: true
        );

        await channel.QueueDeclareAsync(
            queue: QueueName,
            // to make queue persistant
            durable: true,
            // specific to per connection only 
            exclusive: false,
            // auto delete after the last consumer disconnects
            autoDelete: false
        );

        await channel.QueueBindAsync(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: RoutingKey
        );
    }
}