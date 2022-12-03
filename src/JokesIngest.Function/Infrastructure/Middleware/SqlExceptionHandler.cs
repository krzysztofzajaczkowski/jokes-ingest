using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace JokesIngest.Function.Infrastructure.Middleware;

public class SqlExceptionHandler : ExceptionHandler<SqliteException>
{
    private readonly ILogger<SqlExceptionHandler> _logger;

    public SqlExceptionHandler(ILogger<SqlExceptionHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(FunctionContext context, SqliteException exception)
    {
        _logger.LogError(
            "Error thrown while ingesting jokes into sqlite database with message: {message}. " +
            "(Exception error code: {errorCode}, InvocationId: {invocationId})",
            exception.Message, exception.SqliteErrorCode, context.InvocationId);

        return Task.CompletedTask;
    }
}