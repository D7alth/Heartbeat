namespace src.Entities.Repositories;

public interface ISensorReadingRepository
{
    Task SaveAsync(SensorReading reading);
    // metric is a better name I guess
}
