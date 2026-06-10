using Microsoft.IdentityModel.Tokens;
using TraineeManagement.Api.Services;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
.AddJsonOptions(options => {options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());} );
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseInMemoryDatabase("TraineeApp"));

//  To get the connection creditials of the MySql
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 46)); 

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

// Use AddSingleton when we are storing in the List 
// For inMemory and the Persistant Database use the AddScoped
builder.Services.AddScoped<ITraineeService, TraineeService>();
var app = builder.Build();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
