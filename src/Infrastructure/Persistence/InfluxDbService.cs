using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.Options;
using src.Contracts;
using src.Infrastructure.Persistence.Connection;
using src.Models;

namespace src.Infrastructure.Persistence;

public sealed class InfluxDbService(
    IDbClientFactory dbClientFactory,
    IOptions<InfluxOptions> options
) : IReadingRepository
{
    private InfluxOptions InfluxOptions => options.Value;

    public async Task SaveAsync(HumidityReading humidityReading)
    {
        try
        {
            var influxDbClient = dbClientFactory.Create();
            var writeApi = influxDbClient.GetWriteApiAsync();
            await writeApi.WriteMeasurementAsync(
                humidityReading,
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

    public Task SaveAsync(PresenceReading presenceReading)
    {
        throw new NotImplementedException();
    }
}
