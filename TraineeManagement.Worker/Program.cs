using TraineeManagement.Messaging;
using TraineeManagement.Worker;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using TraineeManagement.Api.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IConnection>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var factory = RabbitMqSetup.GetConnectionFactory(config);

    var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();

    RabbitMqSetup.InitializeTopologyAsync(connection).GetAwaiter().GetResult();

    return connection;
});

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
MySqlServerVersion serverVersion = new MySqlServerVersion(new Version(8, 0, 46));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

// Register the consumer worker
builder.Services.AddHostedService<SubmissionConsumerWorker>();

var host = builder.Build();
host.Run();