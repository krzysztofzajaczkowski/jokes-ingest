using JokesIngest;
using JokesIngest.Filters;
using JokesIngest.Function.Configuration;
using JokesIngest.Provider;
using JokesIngest.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(config =>
        config.SetBasePath(Directory.GetCurrentDirectory())
#if DEBUG
            .AddJsonFile("local.settings.json", true)
#endif
            .AddEnvironmentVariables())
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton(provider =>
        {
            var section = provider.GetRequiredService<IConfiguration>().GetSection("JokesProvider");
            return new JokesProviderConfiguration(
                section.GetValue<int>("BatchSize"),
                section.GetValue<string>("JokeResourcePath"));
        });
        services.AddSingleton(provider => new JokesRepositoryConfiguration(
            provider.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection")));

        services.AddScoped<IJokeFilter, RegexFilter>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            return new RegexFilter(config.GetValue<string>("JokeRegexFilter"));
        });

        services.Configure<JokesApiConfiguration>(context.Configuration.GetSection("JokesProvider"));
        services.AddHttpClient<ApiJokesProvider>((provider, client) =>
        {
            var apiConfiguration = provider.GetRequiredService<IOptions<JokesApiConfiguration>>().Value;
            client.BaseAddress = apiConfiguration.BaseAddress;
            foreach (var (key, value) in apiConfiguration.Headers)
            {
                client.DefaultRequestHeaders.Add(key, value);
            }
        });
        services.AddScoped<IJokesProvider>(provider => provider.GetRequiredService<ApiJokesProvider>());

        services.AddScoped<SqliteJokesRepository>();
        services.AddScoped<IJokesSaver, SqliteJokesRepository>();

        services.AddScoped<JokesProcessor>();
    })
    .Build();

using var startupScope = host.Services.CreateScope();
await startupScope.ServiceProvider.GetRequiredService<SqliteJokesRepository>().EnsureDatabaseCreated();

await host.RunAsync();