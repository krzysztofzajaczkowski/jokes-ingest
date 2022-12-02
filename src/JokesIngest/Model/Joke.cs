using JokesIngest.Exceptions;
using JokesIngest.Utils;

namespace JokesIngest.Model
{
    public class Joke
    {
        public string Hash { get; }
        public string Text { get; }

        private Joke(string hash, string text)
        {
            Hash = hash;
            Text = text;
        }

        public static Joke From(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new InvalidJokeException();
            }

            var hash = Hasher.HashToBase64(text);
            return new Joke(hash, text);
        }
    }
}