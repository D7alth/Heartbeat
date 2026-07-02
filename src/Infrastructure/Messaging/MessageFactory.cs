using System.Buffers;
using System.Text;
using MQTTnet;

namespace src.Infrastructure.Messaging;

public static class MessageFactory
{
    public static Message CreateMessageFromMqtt(MqttApplicationMessageReceivedEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);
        ArgumentNullException.ThrowIfNull(args.ApplicationMessage);
        return new Message(
            ParsePayloadToString(args.ApplicationMessage.Payload),
            args.ClientId,
            args.PacketIdentifier,
            args.ApplicationMessage.Topic
        );
    }

    private static string ParsePayloadToString(ReadOnlySequence<byte> payload) =>
        Encoding.UTF8.GetString(payload);
}
