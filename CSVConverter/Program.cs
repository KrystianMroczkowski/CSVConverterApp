using CSVConverter;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
	.ConfigureFunctionsWorkerDefaults()
	.ConfigureServices(services =>
	{
		services.AddApplicationInsightsTelemetryWorkerService();
		services.ConfigureFunctionsApplicationInsights();
	})
	.ConfigureAppConfiguration(c =>
	{
		c.AddJsonFile("appsettings.json", optional: true);
		c.AddJsonFile("local.settings.json", optional: true);
		c.AddEnvironmentVariables();
	})
	.Build();

host.Run();
