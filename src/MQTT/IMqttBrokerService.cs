namespace src.MQTT;

public interface IMqttBrokerService
{
    Task<Message> ConsumeMessage();
    // TODO: Think about something like "EmptyTopic", you know what I talk about : )
}
