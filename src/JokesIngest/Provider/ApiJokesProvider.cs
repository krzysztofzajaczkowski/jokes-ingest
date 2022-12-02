using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using JokesIngest.Model;

namespace JokesIngest.Provider
{
    public class ApiJokesProvider : IJokesProvider
    {
        private readonly HttpClient _httpClient;
        private readonly JokesProviderConfiguration _jokesProviderConfiguration;

        public ApiJokesProvider(HttpClient httpClient, JokesProviderConfiguration jokesProviderConfiguration)
        {
            _httpClient = httpClient;
            _jokesProviderConfiguration = jokesProviderConfiguration;
        }

        public async IAsyncEnumerable<Joke> GetJokesAsync()
        {
            for (var i = 0; i < _jokesProviderConfiguration.BatchSize; ++i)
            {
                using var response = await _httpClient.GetAsync(_jokesProviderConfiguration.JokeResourcePath);
                response.EnsureSuccessStatusCode();

                yield return (await response.Content.ReadFromJsonAsync<Joke>())!;
            }
        }
    }
}