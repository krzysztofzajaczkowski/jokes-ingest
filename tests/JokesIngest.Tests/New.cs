using JokesIngest.Model;

namespace JokesIngest.Tests;

internal static class New
{
    public static Joke Joke(string id = null, string iconUrl = null, string url = null, string value = null) =>
        new(id!, iconUrl!, url!, value!);
}