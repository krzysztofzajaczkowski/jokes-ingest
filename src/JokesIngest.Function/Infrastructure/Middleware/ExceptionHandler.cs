using JokesIngest.Function.Infrastructure.ErrorHandlers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace JokesIngest.Function.Infrastructure.Middleware;

public class ExceptionHandler : IFunctionsWorkerMiddleware
{
    private readonly IEnumerable<IErrorHandler> _errorHandlers;

    public ExceptionHandler(IEnumerable<IErrorHandler> errorHandlers)
    {
        _errorHandlers = errorHandlers;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        // TODO Change to typed exception handlers after https://github.com/Azure/azure-functions-dotnet-worker/issues/993
        // As of now, exceptions in async operations are wrapped in AggregateException. Azure Functions .NET worker team plans on changing this behavior in the next major version.
        // After that change I would create typed handlers for specific exceptions and configure it like a standard middleware pipe.
        try
        {
            await next(context);
        }
        catch (AggregateException e)
        {
            foreach (var innerException in e.InnerExceptions)
            {
                var handler = _errorHandlers.FirstOrDefault(x => x.CanHandle(innerException));

                if (handler == null)
                {
                    throw;
                }

                await handler.Handle(context, innerException);
            }
        }
    }
}