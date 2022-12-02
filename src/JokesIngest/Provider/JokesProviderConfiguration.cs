namespace JokesIngest.Provider
{
    public class JokesProviderConfiguration
    {
        public int BatchSize { get; }
        public string JokeResourcePath { get; }

        public JokesProviderConfiguration(int batchSize, string jokeResourcePath)
        {
            BatchSize = batchSize;
            JokeResourcePath = jokeResourcePath;
        }
    }
}