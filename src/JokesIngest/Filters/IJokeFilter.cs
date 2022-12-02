using JokesIngest.Model;

namespace JokesIngest.Filters
{
    public interface IJokeFilter
    {
        bool SatisfiedBy(Joke joke);
    }
}