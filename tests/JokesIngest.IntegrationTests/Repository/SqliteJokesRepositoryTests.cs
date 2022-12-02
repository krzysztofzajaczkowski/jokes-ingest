using JokesIngest.Model;
using JokesIngest.Repository;
using JokesIngest.Tests;
using Machine.Specifications;

// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers

namespace JokesIngest.IntegrationTests.Repository;

[Subject(typeof(SqliteJokesRepository))]
public class SqliteJokesRepositoryTests
{
    const string DataSource = "Data Source=:memory:";
    
    private static SqliteJokesRepository repository;
    private static IEnumerable<Joke> jokes;

    private Establish ctx = async () =>
    {
        repository = new SqliteJokesRepository(new JokesRepositoryConfiguration(DataSource));
        await repository.EnsureDatabaseCreated();
    };

    private Because of = async () => await repository.SaveJokes(jokes.ToAsyncEnumerable());
    

    class When_empty_jokes_collection
    {
        private Establish ctx = () => jokes = Enumerable.Empty<Joke>();

        private It should_not_save_any_jokes = async () =>
        {
            var savedJokes = await repository.GetJokesAsync();
            savedJokes.ShouldBeEmpty();
        };
    }

    class When_non_empty_jokes_collection
    {
        private Establish ctx = () => jokes = new[]
        {
            New.Joke(id: "1", iconUrl: "id1-icon-url", url: "id1-url", value: "A joke"),
            New.Joke(id: "2", iconUrl: "id2-icon-url", url: "id2-url", value: "Another joke"),
            New.Joke(id: "3", iconUrl: "id3-icon-url", url: "id3-url", value: "Next Chuck joke")
        };

        private It should_save_jokes = async () =>
        {
            var savedJokes = await repository.GetJokesAsync();
            savedJokes.ShouldEqual(jokes);
        };
    }
}