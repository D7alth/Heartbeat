// See https://aka.ms/new-console-template for more information

using Heartbeat.Producer.Core.Contracts;
using Heartbeat.Producer.Infrastructure.Services;
using Heartbeat.Producer.Infrastructure.Services.EventPublish;
using Heartbeat.Producer.Infrastructure.Stubs.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var builder = new HostApplicationBuilder();
builder.Services.AddSingleton<IMetricCollector, MetricCollectorStub>();
builder.Services.Configure<PublisherOptions>(builder.Configuration.GetSection("PublisherOptions"));
builder.Services.AddScoped<IPublisher, Publisher>(opt => new (
    opt.GetRequiredService<IOptions<PublisherOptions>>().Value
));
builder.Services.AddHostedService<ServiceWorker>(opt => new (
    builder.Services.BuildServiceProvider().GetRequiredService<IMetricCollector>(),
    builder.Services.BuildServiceProvider().GetRequiredService<IPublisher>(),
    builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ServiceWorker>>()
));
var app = builder.Build();
app.Run();
