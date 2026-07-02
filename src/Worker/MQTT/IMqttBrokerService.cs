using src.Infrastructure.Messaging;

namespace src.Worker.MQTT;

public interface IMqttBrokerService
{
    Task<Message> Consume(CancellationToken cancellationToken);
    // TODO: Think about something like "EmptyTopic", you know what I talk about : )
}
