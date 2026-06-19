using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.Options;
using src.Contracts;
using src.Models;

namespace src.Infrastructure;

public class InfluxReadingRepository(InfluxDBClient influxDbClient, IOptions<InfluxOptions> options)
    : IReadingRepository
{
    private InfluxOptions InfluxOptions => options.Value;

    public async Task SaveAsync(HumidityReading humidityReading)
    {
        try
        {
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
