using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace src.MQTT;

public sealed class MqttConnectionFactory(IOptions<MqttBrokerOptions> options, ILogger logger)
{
    private readonly MqttBrokerOptions _valueOptions = options.Value;

    public async Task<bool> TryConnect()
    {
        try
        {
            var factory = new MqttClientFactory();
            var mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(_valueOptions.Broker, _valueOptions.Port)
                .WithCredentials(_valueOptions.Username, _valueOptions.Password)
                .WithClientId(_valueOptions.ClientId)
                .WithCleanSession()
                .WithTlsOptions(builder =>
                {
                    builder.UseTls();
                    builder.WithSslProtocols(SslProtocols.Tls12);
                    builder.WithClientCertificates(
                        new List<X509Certificate2>()
                        {
                            X509CertificateLoader.LoadCertificateFromFile(
                                _valueOptions.TslCertificatePath
                            ),
                        }
                    );
                })
                .Build();
            var connection = await mqttClient.ConnectAsync(options);
            if (connection.ResultCode == MqttClientConnectResultCode.Success)
                return true;
            logger.LogError("Cannot be connect, throwing an exception");
            throw new Exception(connection.ResponseInformation);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw;
        }
    }
}
