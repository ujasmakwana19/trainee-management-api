using Microsoft.IdentityModel.Tokens;
using TraineeManagement.Api.TraineeServices;
using TraineeManagement.Api.UserServices;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.JwtServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using TraineeManagement.Api.ExceptionMiddlewares;
using TraineeManagement.Api.MentorServices;
using TraineeManagement.Api.LearningTaskServices;
using TraineeManagement.Api.TrackTaskService;
using TraineeManagement.Api.SubmissionService;
using TraineeManagement.Api.ReviewService;
using Microsoft.OpenApi.Models;

String MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Setup phase builder
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// 
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000",
                                              "http://localhost:5173");
                      });
});


// System.Text.Json deserializes enums from their integer value by default.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
            // This method JsonStringEnumConverter adds string support on top — it doesn't remove int support.
            // new System.Text.Json.Serialization.JsonStringEnumConverter()

            // For only string support from the Frontend
            new System.Text.Json.Serialization.JsonStringEnumConverter(
                System.Text.Json.JsonNamingPolicy.CamelCase,
                allowIntegerValues: false
            )
        );
});



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Swagger Configuration
builder.Services.AddOpenApi("v1", options =>
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
});
// -----------------------------------------

//--------------- DB ------------------------
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseInMemoryDatabase("TraineeApp"));

//  To get the connection creditials of the MySql
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
MySqlServerVersion serverVersion = new MySqlServerVersion(new Version(8, 0, 46));

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
builder.Services.AddScoped<ITraineeService, TraineeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IMentorService, MentorService>();
builder.Services.AddScoped<ILearningTaskService, LearningTaskService>();
builder.Services.AddScoped<ITrackTaskService, TrackTaskService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IReviewService, ReviewService>();


builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();


WebApplication app = builder.Build();

// Seeder Function
await SeederService.CreateAdminUser(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
