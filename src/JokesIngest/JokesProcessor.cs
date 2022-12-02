using System.Collections.Generic;
using System.Threading.Tasks;
using JokesIngest.Filters;
using JokesIngest.Provider;
using JokesIngest.Repository;
using JokesIngest.Utils;

namespace JokesIngest
{
    public class JokesProcessor
    {
        private readonly IJokesProvider _jokesProvider;
        private readonly IJokesSaver _jokesSaver;
        private readonly IEnumerable<IJokeFilter> _filters;

        public JokesProcessor(IJokesProvider jokesProvider, IJokesSaver jokesSaver, IEnumerable<IJokeFilter> filters)
        {
            _jokesProvider = jokesProvider;
            _jokesSaver = jokesSaver;
            _filters = filters;
        }

        public async Task IngestNextBatch()
        {
            var jokes = _jokesProvider.GetJokesAsync();

            jokes = jokes.ApplyFilters(_filters);

            await _jokesSaver.SaveJokes(jokes);
        }
    }
}