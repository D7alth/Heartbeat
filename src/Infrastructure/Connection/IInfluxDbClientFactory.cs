using InfluxDB.Client;

namespace src.Infrastructure.Connection;

public interface IInfluxDbClientFactory
{
    InfluxDBClient Create();
    void Dispose();
}
