// See https://aka.ms/new-console-template for more information

using Heartbeat.Producer.Core.Contracts;
using Heartbeat.Producer.Infrastructure.Services;
using Heartbeat.Producer.Infrastructure.Stubs.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = new HostApplicationBuilder();
builder.Services.AddSingleton<IMetricCollector, MetricCollectorStub>();
builder.Services.AddHostedService<ServiceWorker>(opt => new ServiceWorker(
    builder.Services.BuildServiceProvider().GetRequiredService<IMetricCollector>(),
    builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ServiceWorker>>()
));
var app = builder.Build();
app.Run();
