using InfluxDB.Client;
using Microsoft.Extensions.Options;
using src.Infrastructure.Configuration;

namespace src.Infrastructure.Persistence.Connection;

public sealed class DbClientFactory(IOptions<InfluxOptions> options) : IDbClientFactory, IDisposable
{
    private readonly InfluxOptions _influxOptions = options.Value;
    private InfluxDBClient? _dbClient;

    public InfluxDBClient Create()
    {
        _dbClient ??= new InfluxDBClient(_influxOptions.Uri, _influxOptions.Token);
        return _dbClient;
    }

    public void Dispose() => _dbClient?.Dispose();
}
