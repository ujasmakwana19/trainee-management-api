using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.JwtServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.Extensions.Http.Resilience;

using TraineeManagement.Api.TraineeServices;
using TraineeManagement.Api.UserServices;
using TraineeManagement.WebCommons.ExceptionMiddlewares;
using TraineeManagement.Api.MentorServices;
using TraineeManagement.Api.LearningTaskServices;
using TraineeManagement.Api.TrackTaskService;
using TraineeManagement.Api.SubmissionService;
using TraineeManagement.Api.ReviewService;
using TraineeManagement.Api.FileServices;
using TraineeManagement.Api.SubmissionFileService;
using TraineeManagement.Data.CacheServices;
using StackExchange.Redis;
using RabbitMQ.Client;
using TraineeManagement.Messaging;
using TraineeManagement.WebCommons.CoorealationIdMiddlewares;
using TraineeManagement.WebCommons.CoorealationIdServices;
using Serilog;
using Serilog.Events;
using TraineeManagement.Data.CacheSetup;
using TraineeManagement.Data.DatabaseSetup;
using TraineeManagement.WebCommons.CorsSetup;
using TraineeManagement.WebCommons.LoggingSetup;
using TraineeManagement.Api.AuthSetup;
using TraineeManagement.Api.HttpServices;


// Setup phase builder
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//-------------- Cors Setup ----------------------
builder.Services.AddCorsConfig(builder.Configuration);
// -----------------------------------------------

//--------- Controller and JSON Options Setup
builder.Services.AddControllers()
.ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    })
.AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.UnmappedMemberHandling = 
        System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow;
    });
// -------------------------------------------------


//Logs Configuration --> To add the logs with the correlationID appended-----------
builder.Host.AddLoggingConfig();
// -----------------------------------------------------------

builder.Services.AddDb(builder.Configuration);

// Authentication--------------------------
builder.Services.AddJwtAuthentication(builder.Configuration);
// ------------------------------------------



// Dependency Injection 
builder.Services.AddScoped<ITraineeService,TraineeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IMentorService, MentorService>();
builder.Services.AddScoped<ILearningTaskService, LearningTaskService>();
builder.Services.AddScoped<ITrackTaskService, TrackTaskService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ISubmissionFileService, SubmissionFileService>();

builder.Services.AddSingleton<IFileStorageService, LocalStorageFileService>();

builder.Services.AddScoped<ICacheService,CacheService>();

builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();

// To make the Redis Soft Dependency for the system
builder.Services.AddRedisCache(builder.Configuration);

builder.Services.AddRabbitMqConnection(builder.Configuration, failFastOnStartup: false);

builder.Services.AddMicroserviceClient(builder.Configuration); 


// -------
WebApplication app = builder.Build();

// Seeder Function
await SeederService.SeedData(app.Services);

app.UseHttpsRedirection();
app.UseCors(CorsServiceExtensions.MyAllowSpecificOrigins);
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging(options => options.LogTemplate());
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
