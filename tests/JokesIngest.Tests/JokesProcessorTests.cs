using Castle.Core.Internal;
using JokesIngest.Filters;
using JokesIngest.Model;
using JokesIngest.Provider;
using JokesIngest.Repository;
using Machine.Fakes;
using Machine.Specifications;

// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers

namespace JokesIngest.Tests;

[Subject(typeof(JokesProcessor))]
public class JokesProcessorTests : WithSubject<JokesProcessor>
{
    static IJokesSaver repository;

    Establish ctx = () =>
    {
        The<IJokeFilter>()
            .WhenToldTo(x => x.SatisfiedBy(Param<Joke>.IsAnything))
            .Return<Joke>(x => x.Value.Length > 1);

        repository = An<IJokesSaver>();
        Configure(x => x.For<IJokesSaver>()
            .Use(repository));

    };

    Because of = async () => await Subject.IngestNextBatch();

    class When_none_jokes_pass_filter
    {
        Establish ctx = () =>
            The<IJokesProvider>()
                .WhenToldTo(x => x.GetJokesAsync())
                .Return(new[]
                {
                    New.Joke(value: "A"),
                    New.Joke(value: "B"),
                    New.Joke(value: "C")
                }.ToAsyncEnumerable());

        It should_call_repository_with_empty_enumerable = () =>
            repository.WasToldTo(x =>
                x.SaveJokes(Param<IAsyncEnumerable<Joke>>.Matches(jokes =>
                    jokes.ToEnumerable().IsNullOrEmpty())));
    }

    class When_some_jokes_pass_filter
    {
        Establish ctx = () =>
            The<IJokesProvider>()
                .WhenToldTo(x => x.GetJokesAsync())
                .Return(new[]
                {
                    New.Joke(value: "A joke"),
                    New.Joke(value: "A"),
                    New.Joke(value: "Another joke"),
                    New.Joke(value: "B"),
                    New.Joke(value: "Next Chuck joke"),
                    New.Joke(value: "C")
                }.ToAsyncEnumerable);         

        It should_call_repository_with_filtered_jokes = () =>
            repository.WasToldTo(x =>
                x.SaveJokes(Param<IAsyncEnumerable<Joke>>
                    .Matches(jokes =>
                        jokes.ToEnumerable().Select(joke => joke.Value).SequenceEqual(new[]
                        {
                            "A joke",
                            "Another joke",
                            "Next Chuck joke"
                        }))));
    }

    class When_all_jokes_pass_filter
    {
        Establish ctx = () =>
            The<IJokesProvider>()
                .WhenToldTo(x => x.GetJokesAsync())
                .Return(new[]
                {
                    New.Joke(value: "A joke"),
                    New.Joke(value: "Another joke"),
                    New.Joke(value: "Next Chuck joke"),
                }.ToAsyncEnumerable);

        It should_call_repository_with_filtered_jokes = () =>
        {
            repository.WasToldTo(x =>
                x.SaveJokes(Param<IAsyncEnumerable<Joke>>
                    .Matches(jokes =>
                        jokes.ToEnumerable().Select(joke => joke.Value).SequenceEqual(new[]
                        {
                            "A joke",
                            "Another joke",
                            "Next Chuck joke"
                        }))));
        };
    }
}