using JokesIngest.Function.Infrastructure.Middleware;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace JokesIngest.Function.Infrastructure;

// This class might be redundant boilerplaiting but I wholeheartedly resent long, unstructured dependencies registration.
public static class HostBuilderExtensions
{
    public static IHostBuilder AddConfiguration(this IHostBuilder host) =>
        host.ConfigureAppConfiguration(config =>
            config.SetBasePath(Directory.GetCurrentDirectory())
#if DEBUG
                .AddJsonFile("local.settings.json", true)
#endif
                .AddEnvironmentVariables());

    public static void SetupMiddleware(this IFunctionsWorkerApplicationBuilder builder) => builder.UseMiddleware<ExceptionHandler>();

    public static IHostBuilder AddLogging(this IHostBuilder host) =>
        host.UseSerilog((context, services, logger) =>
            logger.Enrich.FromLogContext()
                .ReadFrom.Configuration(context.Configuration)
                .WriteTo.ApplicationInsights(
                    services.GetRequiredService<TelemetryConfiguration>(),
                    TelemetryConverter.Traces)
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:l} {NewLine}{Properties}{NewLine}{Exception}"));
}