using System.Text.RegularExpressions;
using JokesIngest.Model;

namespace JokesIngest.Filters
{
    public class RegexFilter : IJokeFilter
    {
        private readonly string _pattern;

        public RegexFilter(string pattern)
        {
            _pattern = pattern;
        }

        public bool SatisfiedBy(Joke joke)
        {
            return Regex.IsMatch(joke.Value, _pattern);
        }
    }
}