using Heartbeat.Producer.Core.Contracts;
using Heartbeat.Producer.Core.Contracts.Messaging;
using Heartbeat.Producer.Infrastructure.Messaging;
using Heartbeat.Producer.Infrastructure.Messaging.Connection;
using Heartbeat.Producer.Infrastructure.Services;
using Heartbeat.Producer.Infrastructure.Stubs.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = new HostApplicationBuilder();
builder
    .Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddSingleton<IMetricCollector, MetricCollectorStub>();

builder.Services.Configure<KafkaConfigurationOptions>(
    builder.Configuration.GetSection("KafkaConfigurationOptions")
);
builder.Services.AddSingleton<IKafkaConnectionManager, KafkaConnectionManager>();
builder.Services.AddSingleton<IPublisher, KafkaPublisher>();
builder.Services.AddHostedService<KafkaConnectionInitializer>();
builder.Services.AddHostedService<ServiceWorker>();

var app = builder.Build();
app.Run();

app.Run();
