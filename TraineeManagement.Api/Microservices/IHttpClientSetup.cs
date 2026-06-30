using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Serilog;

namespace TraineeManagement.Api.HttpServices;

public static class HttpClientServiceExtensions
{
    public static IServiceCollection AddMicroserviceClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string clientName = configuration["TraineeMicroService:NAME"]
            ?? throw new InvalidOperationException("TraineeMicroService:NAME is missing in configuration.");

        string uri = configuration["TraineeMicroService:URI"]
            ?? throw new InvalidOperationException("TraineeMicroService:URI is missing in configuration.");

        string? userAgent = configuration["TraineeMicroService:USERAGENT"];

        services.AddHttpClient(clientName, client =>
            {
                client.BaseAddress = new Uri(uri);
                if (!string.IsNullOrEmpty(userAgent))
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                }
            })
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);

                options.Retry.OnRetry = args =>
                {
                    Log.Warning(
                        "Retry attempt {AttemptNumber} for {ClientName} after {Delay}ms. Reason: {Reason}",
                        args.AttemptNumber + 1,
                        clientName,
                        args.RetryDelay.TotalMilliseconds,
                        args.Outcome.Exception?.Message ?? args.Outcome.Result?.StatusCode.ToString());

                    return default;
                };
            });

        services.AddScoped<IInterServiceHttpClient, InterServiceHttpClient>();

        return services;
    }
}