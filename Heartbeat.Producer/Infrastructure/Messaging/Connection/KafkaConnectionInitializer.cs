using Confluent.Kafka;
using Heartbeat.Producer.Core.Models.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Heartbeat.Producer.Infrastructure.Messaging.Connection;

internal sealed class KafkaConnectionInitializer(
    IKafkaConnectionManager connectionManager,
    IOptions<KafkaConfigurationOptions> options,
    ILogger<KafkaConnectionInitializer> logger
) : IHostedService
{
    private readonly KafkaConfigurationOptions _options = options.Value;
    private CancellationTokenSource _cts = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _ = RunConnectionLoopAsync(_cts.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken) => await _cts.CancelAsync();

    private async Task RunConnectionLoopAsync(CancellationToken ct)
    {
        var pipeline = BuildRetryPipeline();
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await pipeline.ExecuteAsync(
                    async token =>
                    {
                        logger.LogInformation(
                            "Kafka: attempting to connect to {Server}...",
                            _options.BootstrapServer
                        );
                        ProbeBroker();
                        var producer = BuildProducer();
                        connectionManager.SetProducer(producer);
                        logger.LogInformation("Kafka: connection established. State → Ready.");
                        await RunHealthCheckLoopAsync(token);
                    },
                    ct
                );
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Kafka: all retry attempts exhausted.");
                break;
            }
        }
    }

    private async Task RunHealthCheckLoopAsync(CancellationToken ct)
    {
        var interval = TimeSpan.FromSeconds(_options.HealthCheckIntervalSeconds);
        while (!ct.IsCancellationRequested && connectionManager.State == ConnectionState.Ready)
        {
            await Task.Delay(interval, ct).ConfigureAwait(false);
            try
            {
                ProbeBroker();
                logger.LogDebug("Kafka: health-check OK.");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Kafka: health-check failed. State → Degraded.");
                connectionManager.MarkDegraded();
                break;
            }
        }
    }

    private ResiliencePipeline BuildRetryPipeline() =>
        new ResiliencePipelineBuilder()
            .AddRetry(
                new RetryStrategyOptions
                {
                    MaxRetryAttempts = int.MaxValue,
                    BackoffType = DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromSeconds(_options.RetryInitialDelaySeconds),
                    MaxDelay = TimeSpan.FromSeconds(_options.RetryMaxDelaySeconds),
                    UseJitter = true,
                    OnRetry = args =>
                    {
                        connectionManager.MarkDegraded();
                        logger.LogWarning(
                            "Kafka: connection attempt {Attempt} failed. Retrying in {Delay:F1}s. ({Message})",
                            args.AttemptNumber + 1,
                            args.RetryDelay.TotalSeconds,
                            args.Outcome.Exception?.Message
                        );
                        return ValueTask.CompletedTask;
                    },
                }
            )
            .Build();

    private void ProbeBroker()
    {
        using var adminClient = new AdminClientBuilder(
            new AdminClientConfig
            {
                BootstrapServers = _options.BootstrapServer,
                SecurityProtocol = GetSecurityProtocol(_options.SecurityProtocol),
            }
        ).Build();
        adminClient.GetMetadata(_options.TopicName, TimeSpan.FromSeconds(5));
    }

    private IProducer<string, byte[]> BuildProducer() =>
        new ProducerBuilder<string, byte[]>(
            new ProducerConfig
            {
                BootstrapServers = _options.BootstrapServer,
                ClientId = _options.ClientId,
                SecurityProtocol = GetSecurityProtocol(_options.SecurityProtocol),
                Acks = GetAcks(_options.Acks),
                MessageTimeoutMs = _options.MessageTimeout,
                BatchNumMessages = _options.BachNumMessages,
                LingerMs = _options.LingerMs,
                CompressionType = GetCompressionType(_options.CompressionType),
            }
        ).Build();

    private static SecurityProtocol GetSecurityProtocol(string protocol) =>
        protocol switch
        {
            "SSL" => SecurityProtocol.Ssl,
            "SASL_PLAINTEXT" => SecurityProtocol.SaslPlaintext,
            "SASL_SSL" => SecurityProtocol.SaslSsl,
            _ => SecurityProtocol.Plaintext,
        };

    private static Acks GetAcks(string? acks) =>
        acks switch
        {
            "Leader" => Acks.Leader,
            "None" => Acks.None,
            _ => Acks.All,
        };

    private static CompressionType GetCompressionType(string? compressionType) =>
        compressionType switch
        {
            "Gzip" => CompressionType.Gzip,
            "Snappy" => CompressionType.Snappy,
            "Lz4" => CompressionType.Lz4,
            _ => CompressionType.None,
        };
}
