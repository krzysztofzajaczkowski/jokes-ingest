namespace JokesIngest.Provider
{
    public interface IJokesProviderConfiguration
    {
        public int BatchSize { get; }
        public string JokeResourcePath { get; }
    }
}