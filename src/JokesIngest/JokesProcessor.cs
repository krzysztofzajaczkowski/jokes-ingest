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
            var jokes = _jokesProvider.GetJokesAsync();

            jokes = jokes.ApplyFilters(_filters);

            _jokesRepository.SaveJokes(jokes);
        }
    }
}