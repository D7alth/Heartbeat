using InfluxDB.Client;

namespace src.Infrastructure.Persistence.Connection;

public interface IDbClientFactory
{
    InfluxDBClient Create();
    void Dispose();
}
