using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using JokesIngest.Model;
using Microsoft.Data.Sqlite;

namespace JokesIngest.Repository;

public class SqliteJokesRepository : IJokesSaver, IDisposable
{
    private readonly JokesRepositoryConfiguration _jokesRepositoryConfiguration;
    private SqliteConnection? _connection;

    public SqliteJokesRepository(JokesRepositoryConfiguration jokesRepositoryConfiguration)
    {
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

            _connection = new SqliteConnection(_jokesRepositoryConfiguration.ConnectionString);
            _connection.Open();

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
            await Connection.ExecuteAsync(@"
                CREATE TABLE Jokes (
                    Id CHAR(22) NOT NULL PRIMARY KEY,
                    IconUrl VARCHAR(100) NOT NULL,
                    Url VARCHAR(100) NOT NULL,
                    Value VARCHAR(200) UNIQUE NOT NULL
                );"
            );
        }
    }

    public async Task SaveJokes(IAsyncEnumerable<Joke> jokes)
    {
        await using var transaction = Connection.BeginTransaction();

        await foreach (var joke in jokes)
        {
            await Connection.ExecuteAsync(
                "INSERT OR IGNORE INTO Jokes(Id, IconUrl, Url, Value) VALUES (@Id, @IconUrl, @Url, @Value);",
                joke, transaction);
        }

        transaction.Commit();
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