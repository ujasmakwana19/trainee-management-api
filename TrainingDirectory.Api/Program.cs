using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TraineeManagement.Data.DataBaseContext;
using TraineeManagement.Contracts.ExceptionMiddlewares;
using TrainingDirectory.TraineeInterface;
using TrainingDirectory.TraineeServices;
using TraineeManagement.Contracts.CoorealationIdMiddlewares;
using Serilog;
using Serilog.Events;

String MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration) 
    .Enrich.FromLogContext() 
   .WriteTo.Conditional(
        evt => evt.Properties.TryGetValue("SourceContext", out LogEventPropertyValue? src) && src.ToString().Contains("RequestLoggingMiddleware"),
        wt => wt.Console(outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level:u3}] [HTTP] [Corr: {CorrelationId}] {Message:lj}{NewLine}{Exception}"))
    
    .WriteTo.Conditional(
        evt => !(evt.Properties.TryGetValue("SourceContext", out LogEventPropertyValue? src) && src.ToString().Contains("RequestLoggingMiddleware")),
        wt => wt.Console(outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level:u3}] [{SourceContext}] [Corr: {CorrelationId}] {Message:lj}{NewLine}{Exception}"))
);

string[] allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins2")
    .Get<string[]>() ?? Array.Empty<string>();
// Cors Setup
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins(allowedOrigins);
                      });
});

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>{
            options.SuppressModelStateInvalidFilter = true;   
        }
    )
    .AddJsonOptions(options => {
            options.JsonSerializerOptions.UnmappedMemberHandling = 
            System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow;
        }
    );


// To add the logs with the correlationID appended
builder.Logging.AddConsole(options =>
{
    options.FormatterName = "simple";
}).AddSimpleConsole(options =>
{
    options.IncludeScopes = true; 
});

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string 'DefaultConnection' not found.");
}
MySqlServerVersion serverVersion = new MySqlServerVersion(new Version(8, 0, 46));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

builder.Services.AddScoped<ITraineeService, TraineeService>();


WebApplication app = builder.Build();

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();