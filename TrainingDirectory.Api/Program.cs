using TraineeManagement.WebCommons.ExceptionMiddlewares;
using TrainingDirectory.TraineeInterface;
using TrainingDirectory.TraineeServices;
using TraineeManagement.WebCommons.CoorealationIdMiddlewares;
using Serilog;
using TraineeManagement.Data.DatabaseSetup;
using TraineeManagement.WebCommons.CorsSetup;
using TraineeManagement.WebCommons.LoggingSetup;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.AddLoggingConfig();
builder.Services.AddCorsConfig(builder.Configuration);

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

builder.Services.AddDb(builder.Configuration);

builder.Services.AddScoped<ITraineeService, TraineeService>();


WebApplication app = builder.Build();

app.UseHttpsRedirection();
app.UseCors(CorsServiceExtensions.MyAllowSpecificOrigins);
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging(options => options.LogTemplate());
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();