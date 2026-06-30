using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace TraineeManagement.Data.CacheSetup;

public static class CacheServiceExtensions
{
    // We had used the this cause is the extension method
    public static IServiceCollection AddRedisCache(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            string? connectionString = configuration.GetConnectionString("RedisConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("RedisConnection connection string is missing in configuration.");
            }

            ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);

            // Below ensures the soft dependency 
            options.AbortOnConnectFail = false;
            options.ConnectTimeout = 3000;
            options.ConnectRetry = 2;
            
            return ConnectionMultiplexer.Connect(options);
        });
        return services;
    }
}
