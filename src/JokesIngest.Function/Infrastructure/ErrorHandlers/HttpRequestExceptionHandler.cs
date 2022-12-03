using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace JokesIngest.Function.Infrastructure.ErrorHandlers;

public class HttpRequestExceptionHandler : IErrorHandler<HttpRequestException>
{
    private readonly ILogger<HttpRequestExceptionHandler> _logger;

    public HttpRequestExceptionHandler(ILogger<HttpRequestExceptionHandler> logger)
    {
        _logger = logger;
    }

    public bool CanHandle(Exception exception) => exception is HttpRequestException;

    public Task Handle(FunctionContext context, Exception exception) =>
        Handle(context, (HttpRequestException) exception);

    public Task Handle(FunctionContext context, HttpRequestException exception)
    {
        _logger.LogError("Error thrown while downloading jokes with message: {message}. " +
                        "(StatusCode: {statusCode}, InvocationId: {invocationId})",
            exception.Message, exception.StatusCode, context.InvocationId);

        return Task.CompletedTask;
    }
}