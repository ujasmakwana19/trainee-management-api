using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TraineeManagement.Data.DataBaseContext;
using TraineeManagement.Contracts.ExceptionMiddlewares;
using TrainingDirectory.TraineeInterface;
using TrainingDirectory.TraineeServices;
using TraineeManagement.Contracts.CoorealationIdMiddlewares;
using TraineeManagement.Contracts.CoorealationIdServices;

String MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();