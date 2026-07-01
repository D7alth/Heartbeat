using InfluxDB.Client;

namespace src.Infrastructure.Connection;

public interface IDbClientFactory
{
    InfluxDBClient Create();
    void Dispose();
}
