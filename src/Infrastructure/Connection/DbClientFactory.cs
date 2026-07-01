using InfluxDB.Client;
using Microsoft.Extensions.Options;

namespace src.Infrastructure.Connection;

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
