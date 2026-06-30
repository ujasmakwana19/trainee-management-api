using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Events;

namespace TraineeManagement.WebCommons.LoggingSetup;

public static class LoggingServiceExtensions
{
    public static IHostBuilder AddLoggingConfig(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Conditional(
                evt => evt.Properties.TryGetValue("SourceContext", out LogEventPropertyValue? src)
                    && src.ToString()!.Contains("RequestLoggingMiddleware"),
                wt => wt.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss}] [{Level:u3}] [HTTP] [Corr: {CorrelationId}] {Message:lj}{NewLine}{Exception}"))
            .WriteTo.Conditional(
                evt => !(evt.Properties.TryGetValue("SourceContext", out LogEventPropertyValue? src)
                    && src.ToString()!.Contains("RequestLoggingMiddleware")),
                wt => wt.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss}] [{Level:u3}] [{SourceContext}] [Corr: {CorrelationId}] {Message:lj}{NewLine}{Exception}")));
    }

    public static RequestLoggingOptions LogTemplate(this RequestLoggingOptions options)
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        return options;
    }
}