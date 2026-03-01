using Heartbeat.Producer.Core.Contracts;
using Heartbeat.Producer.Infrastructure.Services;
using Heartbeat.Producer.Infrastructure.Services.EventPublish;
using Heartbeat.Producer.Infrastructure.Stubs.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var builder = new HostApplicationBuilder();
builder
    .Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddSingleton<IMetricCollector, MetricCollectorStub>();
builder.Services.Configure<PublisherOptions>(builder.Configuration.GetSection("PublisherOptions"));
builder.Services.AddScoped<IPublisher>(opt => new Publisher(
    opt.GetRequiredService<IOptions<PublisherOptions>>().Value
));
builder.Services.AddHostedService<ServiceWorker>(opt =>
    new(
        opt.GetRequiredService<IMetricCollector>(),
        opt.GetRequiredService<IPublisher>(),
        opt.GetRequiredService<ILogger<ServiceWorker>>()
    )
);
var app = builder.Build();
app.Run();
