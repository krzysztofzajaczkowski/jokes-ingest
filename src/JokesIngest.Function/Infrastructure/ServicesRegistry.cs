using JokesIngest.Filters;
using JokesIngest.Function.Configuration;
using JokesIngest.Function.Infrastructure.ErrorHandlers;
using JokesIngest.Provider;
using JokesIngest.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace JokesIngest.Function.Infrastructure;

// This class might be redundant boilerplaiting but I wholeheartedly resent long, unstructured dependencies registration.
public static class ServicesRegistry
{
    public static IServiceCollection RegisterConfiguration(this IServiceCollection services, HostBuilderContext context)
    {
        services.Configure<ApplicationConfiguration>(configuration =>
        {
            context.Configuration.GetSection("JokesProvider").Bind(configuration);
            configuration.ConnectionString = context.Configuration.GetConnectionString("DefaultConnection");
        });
        services.AddSingleton<IJokesProviderConfiguration>(provider => provider.GetRequiredService<IOptions<ApplicationConfiguration>>().Value);
        services.AddSingleton<IJokesRepositoryConfiguration>(provider => provider.GetRequiredService<IOptions<ApplicationConfiguration>>().Value);

        return services;
    }

    public static IServiceCollection RegisterApiClient(this IServiceCollection services)
    {
        services.AddHttpClient<ApiJokesProvider>((provider, client) =>
        {
            var apiConfiguration = provider.GetRequiredService<IOptions<ApplicationConfiguration>>().Value;
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
        var handlerImplementations = typeof(Program).Assembly
            .GetTypes()
            .Where(x => !x.IsInterface && x.IsAssignableTo(typeof(IErrorHandler)))
            .ToList();

        foreach (var type in handlerImplementations)
        {
            services.AddSingleton(typeof(IErrorHandler), type);
        }

        return services;
    }
}