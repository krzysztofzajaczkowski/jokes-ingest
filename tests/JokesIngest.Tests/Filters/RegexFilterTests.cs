using System.Diagnostics;
using JokesIngest.Filters;
using JokesIngest.Model;
using Machine.Specifications;

// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers

namespace JokesIngest.Tests.Filters;

[Subject(typeof(RegexFilter), "max 10 characters filter")]
public class RegexFilterTests
{
    private static Joke joke;
    private static IJokeFilter filter;
    private static bool result;

    Establish ctx = () => filter = new RegexFilter(@"^.{0,10}$");

    private Because of = () => result = filter.SatisfiedBy(joke);

    class When_joke_shorter_than_10_characters
    {
        Establish ctx = () => joke = Joke.From("Joke");

        It should_satisfy_filter = () => result.ShouldBeTrue();
    }

    class When_joke_exactly_10_characters
    {
        Establish ctx = () => joke = Joke.From("Short joke");

        It should_satisfy_filter = () => result.ShouldBeTrue();
    }

    class When_joke_longer_than_10_characters
    {
        Establish ctx = () => joke = Joke.From("Longer joke");

        It should_not_satisfy_filter = () => result.ShouldBeFalse();
    }
}