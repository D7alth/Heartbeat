namespace src.Infrastructure.Configuration;

public sealed class InfluxOptions
{
    public required string Token { get; init; }
    public required string Bucket { get; init; }
    public required string Organization { get; init; }
    public required string Uri { get; init; }
};
