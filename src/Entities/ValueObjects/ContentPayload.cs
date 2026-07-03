namespace src.Entities.ValueObjects;

public sealed record ContentPayload
{
    public string RawMessage { get; private set; } = string.Empty;
    public string Topic { get; private set; } = string.Empty;

    private ContentPayload(string rawMessage, string topic)
    {
        RawMessage = rawMessage;
        Topic = topic;
    }

    public static ContentPayload Create(string rawMessage, string topic, string firmware)
    {
        ArgumentException.ThrowIfNullOrEmpty(rawMessage);
        ArgumentException.ThrowIfNullOrEmpty(topic);
        ArgumentException.ThrowIfNullOrEmpty(firmware);
        return new ContentPayload(rawMessage, topic);
    }
}
