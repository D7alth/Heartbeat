using src.Entities.ValueObjects;

namespace src.Entities;

public sealed class SensorReading
{
    public Guid RecordId { get; private set; }
    public DateTime? RecordedAt { get; private set; }
    public TimeSpan Uptime { get; private set; }
    public int RssiDbm { get; private set; }
    public SensorInformation Sensor { get; private set; }
    public ContentPayload ContentPayload { get; private set; }

    private SensorReading(
        DateTime? recordedAt,
        TimeSpan uptime,
        int rssiDbm,
        SensorInformation sensor,
        ContentPayload contentPayload
    )
    {
        RecordId = Guid.CreateVersion7();
        RecordedAt = recordedAt;
        Uptime = uptime;
        RssiDbm = rssiDbm;
        Sensor = sensor;
        ContentPayload = contentPayload;
    }

    public static SensorReading Create(
        DateTime recordedAt,
        TimeSpan uptime,
        int rssiDbm,
        SensorInformation sensor,
        ContentPayload contentPayload
    ) => new(recordedAt, uptime, rssiDbm, sensor, contentPayload);
}
