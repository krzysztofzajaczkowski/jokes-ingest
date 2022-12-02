using System.Text.Json.Serialization;

namespace JokesIngest.Model
{
    public record Joke(string Id, [property:JsonPropertyName("icon_url")] string IconUrl, string Url, string Value);
}