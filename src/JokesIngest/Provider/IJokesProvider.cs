using System.Collections.Generic;
using System.Threading.Tasks;
using JokesIngest.Model;

namespace JokesIngest.Provider
{
    public interface IJokesProvider
    {
        IAsyncEnumerable<Joke> GetJokesAsync();
    }
}