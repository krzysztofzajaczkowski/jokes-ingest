using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using JokesIngest.Model;
using Microsoft.Extensions.Logging;

namespace JokesIngest.Provider
{
    public class ApiJokesProvider : IJokesProvider
    {
        private readonly ILogger<ApiJokesProvider> _logger;
        private readonly HttpClient _httpClient;
        private readonly IJokesProviderConfiguration _jokesProviderConfiguration;

        public ApiJokesProvider(ILogger<ApiJokesProvider> logger, HttpClient httpClient, IJokesProviderConfiguration jokesProviderConfiguration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _jokesProviderConfiguration = jokesProviderConfiguration;
        }

        public async IAsyncEnumerable<Joke> GetJokesAsync()
        {
            for (var i = 0; i < _jokesProviderConfiguration.BatchSize; ++i)
            {
                _logger.LogDebug("Retrieving joke #{number} out of {batchSize}.", 
                    i + 1, _jokesProviderConfiguration.BatchSize);
                using var response = await _httpClient.GetAsync(_jokesProviderConfiguration.JokeResourcePath);
                response.EnsureSuccessStatusCode();

                yield return (await response.Content.ReadFromJsonAsync<Joke>())!;
            }
        }
    }
}