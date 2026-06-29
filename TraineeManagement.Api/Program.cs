using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.JwtServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.Extensions.Http.Resilience;

using TraineeManagement.Api.TraineeServices;
using TraineeManagement.Api.UserServices;
using TraineeManagement.Data.DataBaseContext;
using TraineeManagement.Contracts.ExceptionMiddlewares;
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
using TraineeManagement.Contracts.CoorealationIdMiddlewares;
using TraineeManagement.Contracts.CoorealationIdServices;
using Serilog;
using Serilog.Events;

String MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Setup phase builder
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Cors Setup
string[] allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins1")
    .Get<string[]>() ?? Array.Empty<string>();
    
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins(allowedOrigins);
                      });
});

// Controller and JSON Options Setup
builder.Services.AddControllers()
// This is to suppress the default model state validation behavior of ASP.NET Core, 
// which automatically returns a 400 Bad Request response if the model state is invalid. 
// By setting SuppressModelStateInvalidFilter to true, you can handle model validation 
// errors manually in your controller actions, allowing for more customized error responses 
// or additional processing before returning a response to the client.
.ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    })

// System.Text.Json deserializes enums from their integer value by default.
// .AddJsonOptions(options =>
// {
//     // options.JsonSerializerOptions.Converters.Add(
//     //         // This method JsonStringEnumConverter adds string support on top — it doesn't remove int support.
//     //         // new System.Text.Json.Serialization.JsonStringEnumConverter()

//     //         // For only string support from the Frontend
//     //         // new System.Text.Json.Serialization.JsonStringEnumConverter(
//     //         //     System.Text.Json.JsonNamingPolicy.CamelCase,
//     //         //     allowIntegerValues: false
//     //         // )
//     //     );
// })
// 
.AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow;
    });

// To add the logs with the correlationID appended
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration) 
    .Enrich.FromLogContext() 
    .WriteTo.Conditional(
        eventHappen => eventHappen.Properties.TryGetValue("SourceContext", out LogEventPropertyValue? src) 
        && src.ToString().Contains("RequestLoggingMiddleware"),

        wt => wt.Console(outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level:u3}] [HTTP] [Corr: {CorrelationId}] {Message:lj}{NewLine}{Exception}")
    )
    .WriteTo.Conditional(
        evt => !(evt.Properties.TryGetValue("SourceContext", out LogEventPropertyValue? src) 
        && src.ToString().Contains("RequestLoggingMiddleware")),
        wt => wt.Console(outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level:u3}] [{SourceContext}] [Corr: {CorrelationId}] {Message:lj}{NewLine}{Exception}")
    )
);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Swagger Configuration
/* builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        OpenApiSecurityScheme scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token directly"
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes.Add("Bearer", scheme);

        // 2. Apply it globally to all endpoints
        document.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }] = Array.Empty<string>()
        });

        return Task.CompletedTask;
    });
}); */
// -----------------------------------------

//--------------- DB ------------------------
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseInMemoryDatabase("TraineeApp"));

//  To get the connection creditials of the MySql
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
MySqlServerVersion serverVersion = new MySqlServerVersion(new Version(8, 0, 41));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));
//--------------------------------------------- 

// Authentication--------------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });
// ------------------------------------------


// Use AddSingleton when we are storing in the List 
// For inMemory and the Persistant Database use the AddScoped



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
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    ConfigurationOptions options = ConfigurationOptions.Parse(
        builder.Configuration.GetConnectionString("RedisConnection")!);
        options.AbortOnConnectFail = false;
        options.ConnectTimeout = 3000;
        options.ConnectRetry = 2;
    return ConnectionMultiplexer.Connect(options);
});

builder.Services.AddSingleton<IConnection>(sp =>
{
    // Hard Dependency for the RabbitMQ Connection
    // ->>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    // ConnectionFactory factory = RabbitMqSetup.GetConnectionFactory(builder.Configuration);
    // IConnection connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();

    // Console.WriteLine("RabbitMQ Connection Established");
    // return connection;

    // Soft Dependency for the RabbitMQ Connection
    
    ConnectionFactory factory = RabbitMqSetup.GetConnectionFactory(builder.Configuration);
    try
    {
        Task<IConnection> connection = factory.CreateConnectionAsync();
        if (connection.Wait(TimeSpan.FromSeconds(5)))
        {
            Console.WriteLine("RabbitMQ Connection Established");
            return connection.Result;
        }
        throw new TimeoutException("RabbitMQ connection timed out after 5s");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"RabbitMQ unavailable at startup: {ex.Message}");
        return null!; 
    }
});

string httpClientName = builder.Configuration["TraineeMicroService:NAME"]!;

builder.Services.AddHttpClient(httpClientName,client => {
        client.BaseAddress = new Uri(builder.Configuration["TraineeMicroService:URI"]!);
        client.DefaultRequestHeaders.UserAgent.ParseAdd(builder.Configuration["TraineeMicroService:USERAGENT"]);
        }
    )
    .AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 3;
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
});

WebApplication app = builder.Build();

// Seeder Function
await SeederService.SeedData(app.Services);

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
//     app.UseSwaggerUi(options =>
//     {
//         options.DocumentPath = "/openapi/v1.json";
//     });
// }

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
