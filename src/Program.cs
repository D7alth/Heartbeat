using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using src.MQTT;
using src.MQTT.Configuration;
using src.MQTT.Connection;

var builder = Host.CreateApplicationBuilder();

builder
    .Services.AddOptions<MqttBrokerOptions>()
    .Bind(builder.Configuration.GetSection("Mqtt"))
    .ValidateOnStart();

builder.Services.AddSingleton<IMqttConnectionManager, MqttConnection>();
builder.Services.AddHostedService<MqttBrokerService>();

var host = builder.Build();

await host.RunAsync();
