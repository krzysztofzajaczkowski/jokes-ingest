using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace JokesIngest.Function.Infrastructure.ErrorHandlers;

public class SqliteExceptionHandler : IErrorHandler<SqliteException>
{
    private readonly ILogger<SqliteExceptionHandler> _logger;

    public SqliteExceptionHandler(ILogger<SqliteExceptionHandler> logger)
    {
        _logger = logger;
    }

    public bool CanHandle(Exception exception) => exception is SqliteException;

    public Task Handle(FunctionContext context, Exception exception) =>
        Handle(context, (SqliteException) exception);

    public Task Handle(FunctionContext context, SqliteException exception)
    {
        _logger.LogError("Error thrown while ingesting jokes into sqlite database with message: {message}. " +
                         "(Sqlite error code: {errorCode}, InvocationId: {invocationId})",
            exception.Message, exception.SqliteErrorCode, context.InvocationId);

        return Task.CompletedTask;
    }
}