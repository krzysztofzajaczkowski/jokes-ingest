using System.Collections.Generic;
using System.Threading.Tasks;
using JokesIngest.Model;

namespace JokesIngest.Provider
{
    public interface IJokesProvider
    {
        Task<IEnumerable<Joke>> GetJokesAsync();
    }
}