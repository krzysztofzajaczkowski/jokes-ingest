using JokesIngest.Provider;
using JokesIngest.Repository;
#pragma warning disable CS8618

namespace JokesIngest.Function.Configuration;

internal class ApplicationConfiguration : IJokesProviderConfiguration, IJokesRepositoryConfiguration
{
    public Uri BaseAddress { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public int BatchSize { get; set; }
    public string JokeResourcePath { get; set; }
    public string ConnectionString { get; set; }
}