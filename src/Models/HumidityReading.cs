using src.Models.Structures;

namespace src.Models;

public class HumidityReading
{
    public string DeviceId { get; private set; }
    public string Firmware { get; private set; }
    public DateTime? RecordedAt { get; private set; }
    public TimeSpan Uptime { get; private set; }
    public int RssiDbm { get; private set; }
    public SensorStruct Sensor { get; private set; } = default;
    public PayloadStruct Payload { get; private set; } = default;

    private HumidityReading(
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

    public static HumidityReading Create(
        string deviceId,
        string firmware,
        DateTime recordedAt,
        TimeSpan uptime,
        int rssiDbm,
        SensorStruct sensor,
        PayloadStruct payload
    ) => new(deviceId, firmware, recordedAt, uptime, rssiDbm, sensor, payload);
}
