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
    static IJokesRepository repository;

    Establish ctx = () =>
    {
        The<IJokeFilter>()
            .WhenToldTo(x => x.SatisfiedBy(Param<Joke>.IsAnything))
            .Return<Joke>(x => x.Text.Length > 1);

        repository = An<IJokesRepository>();
        Configure(x => x.For<IJokesRepository>()
            .Use(repository));

    };

    Because of = async () => await Subject.IngestNextBatch();

    class When_none_jokes_pass_filter
    {
        Establish ctx = () =>
            The<IJokesProvider>()
                .WhenToldTo(x => x.GetJokesAsync())
                .Return(Task.FromResult(new[]
                {
                    Joke.From("A"),
                    Joke.From("B"),
                    Joke.From("C")
                }.AsEnumerable()));

        It should_call_repository_with_empty_enumerable = () =>
            repository.WasNotToldTo(x =>
                x.SaveJokes(Param<IEnumerable<Joke>>.IsAnything));
    }

    class When_some_jokes_pass_filter
    {
        Establish ctx = () =>
            The<IJokesProvider>()
                .WhenToldTo(x => x.GetJokesAsync())
                .Return(Task.FromResult(new[]
                {
                    Joke.From("A joke"),
                    Joke.From("A"),
                    Joke.From("Another joke"),
                    Joke.From("B"),
                    Joke.From("Next Chuck joke"),
                    Joke.From("C")
                }.AsEnumerable()));         

        It should_call_repository_with_filtered_jokes = () =>
            repository.WasToldTo(x =>
                x.SaveJokes(Param<IEnumerable<Joke>>
                    .Matches(jokes =>
                        jokes.Select(joke => joke.Text).SequenceEqual(new[]
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
                .Return(Task.FromResult(new[]
                {
                    Joke.From("A joke"),
                    Joke.From("Another joke"),
                    Joke.From("Next Chuck joke"),
                }.AsEnumerable()));

        It should_call_repository_with_filtered_jokes = () =>
        {
            repository.WasToldTo(x =>
                x.SaveJokes(Param<IEnumerable<Joke>>
                    .Matches(jokes =>
                        jokes.Select(joke => joke.Text).SequenceEqual(new[]
                        {
                            "A joke",
                            "Another joke",
                            "Next Chuck joke"
                        }))));
        };
    }
}