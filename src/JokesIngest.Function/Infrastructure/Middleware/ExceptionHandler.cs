using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace JokesIngest.Function.Infrastructure.Middleware;

public abstract class ExceptionHandler<TException> : IFunctionsWorkerMiddleware where TException : Exception
{
    public abstract Task Handle(FunctionContext context, TException exception);

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (TException exception)
        {
            await Handle(context, exception);
        }
    }
}