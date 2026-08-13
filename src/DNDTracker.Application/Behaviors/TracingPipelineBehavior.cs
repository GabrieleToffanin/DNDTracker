using System.Diagnostics;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.SharedKernel.Queries;
using MediatR;

namespace DNDTracker.Application.Behaviors;

public sealed class TracingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public static readonly ActivitySource ActivitySource = new("DNDTracker.Application");

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;
        string handlerType = request is ICommand or ICommand<TResponse>
            ? "command"
            : request is IQuery<TResponse>
                ? "query"
                : "request";

        using Activity? activity = ActivitySource.StartActivity(
            requestName,
            ActivityKind.Internal);

        activity?.SetTag("mediatR.request.name", requestName);
        activity?.SetTag("mediatR.request.type", handlerType);
        activity?.SetTag("mediatR.request.fullName", typeof(TRequest).FullName);

        try
        {
            TResponse response = await next();
            activity?.SetStatus(ActivityStatusCode.Ok);
            return response;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("exception.type", ex.GetType().Name);
            activity?.SetTag("exception.message", ex.Message);
            throw;
        }
    }
}
