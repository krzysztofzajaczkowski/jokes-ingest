using Microsoft.Azure.Functions.Worker;

namespace JokesIngest.Function.Infrastructure.ErrorHandlers;

public interface IErrorHandler
{
    bool CanHandle(Exception exception);
    Task Handle(FunctionContext context, Exception exception);
}

public interface IErrorHandler<in TException> : IErrorHandler where TException : Exception
{
    Task Handle(FunctionContext context, TException exception);
}