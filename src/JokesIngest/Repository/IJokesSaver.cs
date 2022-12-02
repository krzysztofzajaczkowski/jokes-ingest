using System.Collections.Generic;
using System.Threading.Tasks;
using JokesIngest.Model;

namespace JokesIngest.Repository
{
    public interface IJokesSaver
    {
        Task SaveJokes(IAsyncEnumerable<Joke> jokes);
    }
}