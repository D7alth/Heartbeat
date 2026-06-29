namespace src.MQTT;

public sealed record Message(
    string Content,
    string ClientId,
    ushort PackageIdentifier,
    string Topic
);
