using System.Collections.Generic;
using JokesIngest.Model;

namespace JokesIngest.Repository
{
    public interface IJokesRepository
    {
        void SaveJokes(IEnumerable<Joke> jokes);
    }
}