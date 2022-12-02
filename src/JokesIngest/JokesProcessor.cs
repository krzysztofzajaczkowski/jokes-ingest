using System.Collections.Generic;
using System.Linq;
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
        private readonly IJokesRepository _jokesRepository;
        private readonly IEnumerable<IJokeFilter> _filters;

        public JokesProcessor(IJokesProvider jokesProvider, IJokesRepository jokesRepository, IEnumerable<IJokeFilter> filters)
        {
            _jokesProvider = jokesProvider;
            _jokesRepository = jokesRepository;
            _filters = filters;
        }

        public async Task IngestNextBatch()
        {
            var jokes = await _jokesProvider.GetJokesAsync();

            jokes = jokes.ApplyFilters(_filters);

            // ReSharper disable PossibleMultipleEnumeration
            // .Any() will move iterator only once to check if collection has any elements. Only first element will be enumerated twice.
            if (jokes.Any())
            {
                _jokesRepository.SaveJokes(jokes);
            }
        }
    }
}