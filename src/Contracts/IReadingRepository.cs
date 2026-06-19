using src.Models;

namespace src.Contracts;

public interface IReadingRepository
{
    Task SaveAsync(HumidityReading humidityReading);
    Task SaveAsync(PresenceReading presenceReading);
}
