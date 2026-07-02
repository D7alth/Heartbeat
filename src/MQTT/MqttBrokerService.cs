using Microsoft.Extensions.Hosting;
using MQTTnet;
using src.Contracts;
using src.Infrastructure;
using src.Models;
using src.Models.Structures;
using src.MQTT.Connection;

namespace src.MQTT;

public sealed class MqttBrokerService(
    IMqttConnectionManager connectionManager,
    IReadingRepository readingRepository
) : BackgroundService
{
    private static IMqttClient? _mqttClient;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _mqttClient ??= await connectionManager.TryGetConnection(stoppingToken);
                if (_mqttClient is null)
                    return;
                _mqttClient.ApplicationMessageReceivedAsync += async e =>
                {
                    var message = MessageFactory.CreateMessageFromMqtt(e);
                    //TODO: move logic to a handler
                    var humidityReading = HumidityReading.Create(
                        "teste 01",
                        "test.01",
                        DateTime.Now,
                        TimeSpan.MaxValue,
                        0,
                        new SensorStruct { Model = "Humidity", Type = "ESP32" },
                        new PayloadStruct { Detect = true, TriggerCount = 0 }
                    );
                    await readingRepository.SaveAsync(humidityReading);
                    await e.AcknowledgeAsync(stoppingToken);
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
