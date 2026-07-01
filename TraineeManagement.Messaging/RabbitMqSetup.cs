using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace TraineeManagement.Messaging;

public static class RabbitMqSetup
{
    // For connecting to the RabbitMQ
    public static ConnectionFactory GetConnectionFactory(IConfiguration configuration)
    {
        IConfiguration section = configuration.GetSection("RabbitMQ");
        if(section is null || 
            string.IsNullOrEmpty(section["HostName"]) || 
            string.IsNullOrEmpty(section["Port"]) ||
            string.IsNullOrEmpty(section["UserName"]) ||
            string.IsNullOrEmpty(section["PassWord"]) ||
            string.IsNullOrEmpty(section["VirtualHost"])
        )
        {
            throw new InvalidOperationException("RabbitMq Credentials Error");
        }
        
        int.TryParse(section["Port"], out int port);
        
        return new ConnectionFactory
        {
            HostName = section["HostName"]!,
            Port = port,
            UserName = section["UserName"]!,
            Password = section["Password"]!,
            VirtualHost = section["VirtualHost"]!,
            // It is used to handle the connection automatically if 
            // if MQ does not exists or the failed to connect via starting up
            AutomaticRecoveryEnabled = true
        };
    }

    public static async Task InitializeTopologyAsync(IConnection connection)
    {
        using IChannel channel = await connection.CreateChannelAsync();
        
        // Dead State Queue
        await channel.ExchangeDeclareAsync(
            exchange : QueueConfig.DeadLetterExchange, 
            type: ExchangeType.Direct, 
            durable: true
        );
        
        await channel.QueueDeclareAsync(
            queue : QueueConfig.DeadLetterQueue, 
            durable: true, 
            exclusive: false, 
            autoDelete: false
        );

        await channel.QueueBindAsync(
            queue : QueueConfig.DeadLetterQueue, 
            exchange : QueueConfig.DeadLetterExchange, 
            routingKey : QueueConfig.DeadLetterRoutingKey
        );

        // Binding configurations
        Dictionary<string, object?> queueArguments = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", QueueConfig.DeadLetterExchange },
            { "x-dead-letter-routing-key", QueueConfig.DeadLetterRoutingKey }
        };
        
        // Main Queue Declare
        await channel.ExchangeDeclareAsync(
            exchange: QueueConfig.SubmissionExchange,
            type: ExchangeType.Direct,
            durable: true
        );

        await channel.QueueDeclareAsync(
            queue: QueueConfig.SubmissionQueue,
            // to make queue persistant
            durable: true,
            // specific to per connection only 
            exclusive: false,
            // auto delete after the last consumer disconnects
            autoDelete: false,
            // config main queue rejects to deadstate
            arguments: queueArguments
        );

        await channel.QueueBindAsync(
            queue: QueueConfig.SubmissionQueue,
            exchange: QueueConfig.SubmissionExchange,
            routingKey: QueueConfig.SubmissionRouting
        );

    }
}