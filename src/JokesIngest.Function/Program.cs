using JokesIngest.Function.Infrastructure;
using JokesIngest.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder => builder.SetupMiddleware())
    .AddConfiguration()
    .AddLogging()
    .ConfigureServices((context, services) =>
        services.RegisterConfiguration(context)
            .AddApplicationInsightsTelemetryWorkerService(context.Configuration)
            .RegisterApiClient()
            .AddApplicationServices()
            .AddErrorHandlers())
    .Build();

using var startupScope = host.Services.CreateScope();
await startupScope.ServiceProvider.GetRequiredService<SqliteJokesRepository>().EnsureDatabaseCreated();

await host.RunAsync();