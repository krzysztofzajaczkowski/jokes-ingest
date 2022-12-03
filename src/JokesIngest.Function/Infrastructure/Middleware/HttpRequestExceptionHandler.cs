using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace JokesIngest.Function.Infrastructure.Middleware;

public class HttpRequestExceptionHandler : ExceptionHandler<HttpRequestException>
{
    private readonly ILogger<HttpRequestExceptionHandler> _logger;

    public HttpRequestExceptionHandler(ILogger<HttpRequestExceptionHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(FunctionContext context, HttpRequestException exception)
    {
        _logger.LogError("Error thrown while downloading jokes with message: {message}. " +
                         "(StatusCode: {statusCode}, InvocationId: {invocationId})",
            exception.Message, exception.StatusCode, context.InvocationId);

        return Task.CompletedTask;
    }
}