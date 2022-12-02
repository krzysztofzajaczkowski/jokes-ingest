namespace JokesIngest.Function.Configuration;

internal class JokesApiConfiguration
{
    public Uri BaseAddress { get; set; } = null!;
    public Dictionary<string, string> Headers { get; set; } = null!;
}