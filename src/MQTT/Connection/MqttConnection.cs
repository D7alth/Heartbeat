using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using src.MQTT.Configuration;

namespace src.MQTT.Connection;

public sealed class MqttConnection(IOptions<MqttBrokerOptions> options, ILogger logger)
    : IMqttConnectionManager
{
    private readonly MqttBrokerOptions _valueOptions = options.Value;

    public async Task<IMqttClient> TryGetConnection(CancellationToken cancellationToken)
    {
        try
        {
            var factory = new MqttClientFactory();
            var mqttClient = factory.CreateMqttClient();
            var options = GetBuiltOptions();
            var connection = await mqttClient.ConnectAsync(options, cancellationToken);
            if (connection.ResultCode == MqttClientConnectResultCode.Success)
            {
                await mqttClient.SubscribeAsync(
                    topic: _valueOptions.Topic, // TODO: Create a specific method to subscribe client to a topic
                    cancellationToken: cancellationToken
                );
                return mqttClient;
            }
            logger.LogError("Cannot be connect, throwing an exception");
            throw new Exception(connection.ResponseInformation);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw;
        }
    }

    private MqttClientOptions GetBuiltOptions() =>
        new MqttClientOptionsBuilder()
            .WithTcpServer(_valueOptions.Broker, _valueOptions.Port)
            .WithCredentials(_valueOptions.Username, _valueOptions.Password)
            .WithClientId(_valueOptions.ClientId)
            .WithCleanSession()
            .WithTlsOptions(builder =>
            {
                builder.UseTls();
                builder.WithSslProtocols(SslProtocols.Tls12);
                builder.WithClientCertificates(
                    new List<X509Certificate2>
                    {
                        X509CertificateLoader.LoadCertificateFromFile(
                            _valueOptions.TslCertificatePath
                        ),
                    }
                );
            })
            .Build();
}
