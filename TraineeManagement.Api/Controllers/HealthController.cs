using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using System.Text.Json; 


namespace TraineeManagement.Api.HealthConfigurations;
 
public static class HealthCheck
{
    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddMySql(
                connectionString: configuration["ConnectionStrings:DefaultConnection"]!,
                name: "mysql",
                tags: ["readiness", "db"])
            .AddRedis(
                redisConnectionString: configuration["ConnectionStrings:RedisConnection"]!,
                name: "redis",
                tags: ["readiness", "cache"])
            .AddRabbitMQ(
                sp => {
                  return sp.GetService<IConnection>()!;
                },
                name: "rabbitmq",
                tags: ["readiness", "messaging"])
            .AddUrlGroup(
                uri: new Uri(configuration["TraineeMicroService:URI"] + "health"),
                name: "directory-service",
                tags: ["readiness", "upstream"]);
 
        return services;
    }
 
    public static WebApplication MapAppHealthChecks(this WebApplication app)
        {
            app.MapHealthChecks("/api/health/live", new HealthCheckOptions
            {
                Predicate = _ => false,
                ResponseWriter = WriteHealthResponse
            });
    
            app.MapHealthChecks("/api/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("readiness"),
                ResponseWriter = WriteHealthResponse
            });
    
            return app;
        }
 
    private static Task WriteHealthResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            Status = report.Status == HealthStatus.Healthy ? "ready" : "unavailable",
            Timestamp = DateTime.UtcNow
        }));
    }
}