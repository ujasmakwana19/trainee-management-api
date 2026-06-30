using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace TraineeManagement.WebCommons.CorsSetup;

public static class CorsServiceExtensions
{
    public static String MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    public static IServiceCollection AddCorsConfig(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string[] allowedOrigins = configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                            policy =>
                            {
                                policy.WithOrigins(allowedOrigins);
                            });
        });
        return services;
    }
}
