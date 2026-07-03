using src.Models.Structures;

namespace src.Entities;

public sealed class SensorReading
{
    public string DeviceId { get; private set; }
    public string Firmware { get; private set; }
    public DateTime? RecordedAt { get; private set; }
    public TimeSpan Uptime { get; private set; }
    public int RssiDbm { get; private set; }
    public SensorStruct Sensor { get; private set; }
    public PayloadStruct Payload { get; private set; }

    private SensorReading(
        string deviceId,
        string firmware,
        DateTime? recordedAt,
        TimeSpan uptime,
        int rssiDbm,
        SensorStruct sensor,
        PayloadStruct payload
    )
    {
        DeviceId = deviceId;
        Firmware = firmware;
        RecordedAt = recordedAt;
        Uptime = uptime;
        RssiDbm = rssiDbm;
        Sensor = sensor;
        Payload = payload;
    }

    public static SensorReading Create(
        string deviceId,
        string firmware,
        DateTime recordedAt,
        TimeSpan uptime,
        int rssiDbm,
        SensorStruct sensor,
        PayloadStruct payload
    ) => new(deviceId, firmware, recordedAt, uptime, rssiDbm, sensor, payload);
}
