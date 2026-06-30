using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace TraineeManagement.Messaging;

public static class RabbitMqServiceExtensions
{
    public static IServiceCollection AddRabbitMqConnection(
        this IServiceCollection services,
        IConfiguration configuration,
        bool failFastOnStartup)
    {
        services.AddSingleton<IConnection>(sp =>
        {
            ConnectionFactory factory = RabbitMqSetup.GetConnectionFactory(configuration);
            
            // Hard Dependency
            if (failFastOnStartup)
            {
                IConnection connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
                RabbitMqSetup.InitializeTopologyAsync(connection).GetAwaiter().GetResult();
                return connection;
            }

            // Soft dependency
            try
            {
                Task<IConnection> connectionTask = factory.CreateConnectionAsync();
                if (!connectionTask.Wait(TimeSpan.FromSeconds(5)))
                {
                    throw new TimeoutException("RabbitMQ connection timed out after 5s.");
                }

                Console.WriteLine("RabbitMQ connection established.");
                return connectionTask.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RabbitMQ unavailable at startup: {ex.Message}");
                throw new InvalidOperationException(
                    "RabbitMQ connection could not be established at startup.", ex);
            }
        });

        return services;
    }
}