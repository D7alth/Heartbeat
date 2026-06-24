using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();
var host = builder.Build();

await host.RunAsync();
