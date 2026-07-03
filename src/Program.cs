using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using src.Contracts;
using src.Infrastructure.Persistence;
using src.Infrastructure.Persistence.Connection;
using src.Worker;
using src.Worker.MQTT.Configuration;
using src.Worker.MQTT.Connection;

var builder = Host.CreateApplicationBuilder();

builder
    .Services.AddOptions<MqttBrokerOptions>()
    .Bind(builder.Configuration.GetSection("Mqtt"))
    .ValidateOnStart();

builder
    .Services.AddOptions<InfluxOptions>()
    .Bind(builder.Configuration.GetSection("InfluxDb"))
    .ValidateOnStart();

builder.Services.AddSingleton<IDbClientFactory, DbClientFactory>();
builder.Services.AddSingleton<IReadingRepository, InfluxDbService>();
builder.Services.AddSingleton<IMqttConnectionManager, MqttConnection>();
builder.Services.AddHostedService<HeartbeatConsumerWorker>();

var host = builder.Build();

await host.RunAsync();
