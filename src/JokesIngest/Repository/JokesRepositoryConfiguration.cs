namespace JokesIngest.Repository;

public class JokesRepositoryConfiguration
{
    public string ConnectionString { get; }

    public JokesRepositoryConfiguration(string connectionString)
    {
        ConnectionString = connectionString;
    }
}