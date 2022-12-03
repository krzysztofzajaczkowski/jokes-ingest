using JokesIngest.Filters;
using JokesIngest.Function.Configuration;
using JokesIngest.Function.Infrastructure.ErrorHandlers;
using JokesIngest.Provider;
using JokesIngest.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace JokesIngest.Function.Infrastructure;

// This class might be redundant boilerplaiting but I wholeheartedly resent long, unstructured dependencies registration.
public static class ServicesRegistry
{
    public static IServiceCollection RegisterConfiguration(this IServiceCollection services, HostBuilderContext context)
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

        services.Configure<JokesApiConfiguration>(context.Configuration.GetSection("JokesProvider"));

        return services;
    }

    public static IServiceCollection RegisterApiClient(this IServiceCollection services)
    {
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

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IJokeFilter, RegexFilter>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            return new RegexFilter(config.GetValue<string>("JokeRegexFilter"));
        });

        services.AddScoped<SqliteJokesRepository>();
        services.AddScoped<IJokesSaver, SqliteJokesRepository>();

        services.AddScoped<JokesProcessor>();

        return services;
    }

    public static IServiceCollection AddErrorHandlers(this IServiceCollection services)
    {
        services.AddSingleton<IErrorHandler, HttpRequestExceptionHandler>();
        services.AddSingleton<IErrorHandler, SqliteExceptionHandler>();

        return services;
    }

    public static IServiceCollection AddLogging(this IServiceCollection services, HostBuilderContext context)
    {
        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(context.Configuration)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:l} {NewLine}{Properties}{NewLine}{Exception}")
            .CreateLogger();

        services.AddLogging(builder => builder.ClearProviders().AddSerilog(logger));

        return services;
    }
}