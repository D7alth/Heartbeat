using InfluxDB.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using src.Contracts;
using src.Infrastructure;
using src.Infrastructure.Connection;
using src.MQTT;
using src.MQTT.Configuration;
using src.MQTT.Connection;

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
builder.Services.AddHostedService<MqttBrokerService>();

var host = builder.Build();

await host.RunAsync();
