using System.Collections.Generic;
using JokesIngest.Model;

namespace JokesIngest.Provider
{
    public interface IJokesProvider
    {
        IAsyncEnumerable<Joke> GetJokesAsync();
    }
}