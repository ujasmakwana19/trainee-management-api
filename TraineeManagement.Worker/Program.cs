using TraineeManagement.Messaging;
using TraineeManagement.Worker;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using TraineeManagement.Api.Data;
using StackExchange.Redis;
using TraineeManagement.Api.CacheServices;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IConnection>(sp =>
{
    IConfiguration config = sp.GetRequiredService<IConfiguration>();
    
    IConnectionFactory factory = RabbitMqSetup.GetConnectionFactory(config);

    IConnection connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();

    RabbitMqSetup.InitializeTopologyAsync(connection).GetAwaiter().GetResult();

    return connection;
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    string configuration = builder.Configuration.GetConnectionString("RedisConnection")!;
    return ConnectionMultiplexer.Connect(configuration);
});

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
MySqlServerVersion serverVersion = new MySqlServerVersion(new Version(8, 0, 46));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

// Register the consumer worker
builder.Services.AddHostedService<SubmissionConsumerWorker>();
builder.Services.AddSingleton<ICacheService,CacheService>();
IHost host = builder.Build();
host.Run();