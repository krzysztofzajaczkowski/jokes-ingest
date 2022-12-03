using JokesIngest.Function.Infrastructure.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace JokesIngest.Function.Infrastructure;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddConfiguration(this IHostBuilder host) =>
        host.ConfigureAppConfiguration(config =>
            config.SetBasePath(Directory.GetCurrentDirectory())
#if DEBUG
                .AddJsonFile("local.settings.json", true)
#endif
                .AddEnvironmentVariables());

    public static void SetupMiddleware(this IFunctionsWorkerApplicationBuilder builder)
    {
        builder.UseMiddleware<ExceptionHandler>();
    }
}