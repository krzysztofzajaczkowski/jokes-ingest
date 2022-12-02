
using System.Collections.Generic;
using System.Linq;
using JokesIngest.Filters;
using JokesIngest.Model;

namespace JokesIngest.Utils
{
    internal static class JokesExtensions
    {
        public static IAsyncEnumerable<Joke> ApplyFilters(this IAsyncEnumerable<Joke> jokes, IEnumerable<IJokeFilter> filters)
        {
            // ReSharper disable once ConvertClosureToMethodGroup
            // In my opinion using method group negatively affects readability
            return filters.Aggregate(jokes, (current, filter) => current.Where(x => filter.SatisfiedBy(x)));
        }
    }
}