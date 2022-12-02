using System.Security.Cryptography;
using System.Text;
using JokesIngest.Exceptions;
using JokesIngest.Model;
using Machine.Specifications;

// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers

namespace JokesIngest.Tests.Model;

[Subject(typeof(Joke))]
public class JokeTests
{
    static string HashText(string text) => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(text)));

    class When_joke_text_invalid
    {
        static string text;
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => Joke.From(text));

        class When_joke_text_is_empty
        {
            Establish ctx = () => text = string.Empty;

            It should_throw_invalid_joke_exception = () => exception.ShouldBeOfExactType<InvalidJokeException>();
        }

        class When_joke_text_is_whitespace
        {
            Establish ctx = () => text = " ";

            It should_throw_invalid_joke_exception = () => exception.ShouldBeOfExactType<InvalidJokeException>();
        }
    }

    class When_joke_text_valid
    {
        static string text;
        static Joke joke;

        Establish ctx = () => text = "If you spell Chuck Norris in Scrabble, you win. Forever.";

        Because of = () => joke = Joke.From(text);

        It should_create_correct_joke_hash = () => joke.Hash.ShouldEqual(HashText(text));
    }

    class When_two_jokes_with_same_text
    {
        static string text;
        static Joke jokeA;
        static Joke jokeB;

        Establish ctx = () => text = "If you spell Chuck Norris in Scrabble, you win. Forever.";

        Because of = () =>
        {
            jokeA = Joke.From(text);
            jokeB = Joke.From(text);
        };

        It should_have_same_hashes = () => jokeA.Hash.ShouldEqual(jokeB.Hash);
    }

    class When_two_jokes_with_different_text
    {
        static string jokeAText;
        static string jokeBText;
        static Joke jokeA;
        static Joke jokeB;

        private Establish ctx = () =>
        {
            jokeAText = "If you spell Chuck Norris in Scrabble, you win. Forever.";
            jokeBText = "Time waits for no man. Unless that man is Chuck Norris.";
        };

        Because of = () =>
        {
            jokeA = Joke.From(jokeAText);
            jokeB = Joke.From(jokeBText);
        };

        It should_have_different_hashes = () => jokeA.Hash.ShouldNotEqual(jokeB.Hash);
    }
}