using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using JokesIngest.Model;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace JokesIngest.Repository;

public class SqliteJokesRepository : IJokesSaver, IDisposable
{
    private readonly ILogger<SqliteJokesRepository> _logger;
    private readonly IJokesRepositoryConfiguration _jokesRepositoryConfiguration;
    private SqliteConnection? _connection;

    public SqliteJokesRepository(ILogger<SqliteJokesRepository> logger, IJokesRepositoryConfiguration jokesRepositoryConfiguration)
    {
        _logger = logger;
        _jokesRepositoryConfiguration = jokesRepositoryConfiguration;
    }

    private SqliteConnection Connection
    {
        get
        {
            if (_connection != null)
            {
                return _connection;
            }

            _logger.LogDebug("Creating connection.");
            _connection = new SqliteConnection(_jokesRepositoryConfiguration.ConnectionString);
            _connection.Open();
            _logger.LogDebug("Connection opened.");

            return _connection;
        }
    }

    public async Task EnsureDatabaseCreated()
    {
        var expectedTableName = "Jokes";
        var tables = await Connection.QueryAsync<string>(
            "SELECT name FROM sqlite_master WHERE type='table' AND name = @TableName;",
            new
            {
                TableName = expectedTableName
            });

        var tableName = tables.FirstOrDefault();

        if (string.IsNullOrEmpty(tableName))
        {
            _logger.LogInformation("Table not found, creating table.");
            await Connection.ExecuteAsync(@"
                CREATE TABLE Jokes (
                    Id CHAR(22) NOT NULL PRIMARY KEY,
                    IconUrl VARCHAR(100) NOT NULL,
                    Url VARCHAR(100) NOT NULL,
                    Value VARCHAR(200) UNIQUE NOT NULL
                );"
            );
            _logger.LogInformation("Table created.");
        }
    }

    public async Task SaveJokes(IAsyncEnumerable<Joke> jokes)
    {
        _logger.LogInformation("Ingesting jokes into sqlite database.");
        _logger.LogDebug("Beginning transaction.");
        await using var transaction = Connection.BeginTransaction();

        await foreach (var joke in jokes)
        {
            _logger.LogDebug("Ingesting joke with Id: {jokeId}", joke.Id);
            var affectedRows = await Connection.ExecuteAsync(
                "INSERT OR IGNORE INTO Jokes(Id, IconUrl, Url, Value) VALUES (@Id, @IconUrl, @Url, @Value);",
                joke, transaction);

            if (affectedRows == 0)
            {
                _logger.LogDebug("Found duplicate with Id: {id} while ingesting, joke ignored.", joke.Id);
            }
        }

        transaction.Commit();
        _logger.LogDebug("Transaction committed");
    }

    public async Task<IEnumerable<Joke>> GetJokesAsync()
    {
        var savedJokes = await Connection.QueryAsync<Joke>("SELECT * FROM Jokes");
        return savedJokes;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}