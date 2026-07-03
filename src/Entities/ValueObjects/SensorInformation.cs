namespace src.Entities.ValueObjects;

public struct SensorInformation
{
    public string Type { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public string Firmware { get; private set; } = string.Empty;

    private SensorInformation(string type, string model, string firmware)
    {
        Type = type;
        Model = model;
        Firmware = firmware;
    }

    public static SensorInformation Create(string type, string model, string firmware)
    {
        ArgumentException.ThrowIfNullOrEmpty(type);
        ArgumentException.ThrowIfNullOrEmpty(model);
        ArgumentException.ThrowIfNullOrEmpty(firmware);
        return new SensorInformation(type, model, firmware);
    }
}
