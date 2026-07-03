using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.Options;
using src.Entities;
using src.Entities.Repositories;
using src.Infrastructure.Configuration;
using src.Infrastructure.Persistence.Connection;

namespace src.Infrastructure.Persistence;

public sealed class InfluxDbService(
    IDbClientFactory dbClientFactory,
    IOptions<InfluxOptions> options
) : ISensorReadingRepository
{
    private InfluxOptions InfluxOptions => options.Value;

    public async Task SaveAsync(SensorReading sensorReading)
    {
        try
        {
            var influxDbClient = dbClientFactory.Create();
            var writeApi = influxDbClient.GetWriteApiAsync();
            await writeApi.WriteMeasurementAsync(
                sensorReading,
                WritePrecision.Ns,
                InfluxOptions.Bucket,
                InfluxOptions.Organization
            );
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
