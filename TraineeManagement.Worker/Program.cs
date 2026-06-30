using TraineeManagement.Messaging;
using TraineeManagement.Worker;
using TraineeManagement.Data.CacheServices;
using TraineeManagement.Data.CacheSetup;
using TraineeManagement.Data.DatabaseSetup;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddRabbitMqConnection(builder.Configuration, failFastOnStartup: true);

builder.Services.AddRedisCache(builder.Configuration);

builder.Services.AddDb(builder.Configuration);

// Register the consumer worker
builder.Services.AddSingleton<ICacheService,CacheService>();
builder.Services.AddHostedService<SubmissionConsumerWorker>();
IHost host = builder.Build();
host.Run();