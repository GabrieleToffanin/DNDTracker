using System.Diagnostics;
using System.Diagnostics.Metrics;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.SharedKernel.Queries;
using MediatR;

namespace DNDTracker.Application.Behaviors;

public sealed class TracingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public static readonly ActivitySource ActivitySource = new("DNDTracker.Application");
    public static readonly Meter Meter = new("DNDTracker.Application");
    private static readonly Counter<long> Requests = Meter.CreateCounter<long>(
        "dndtracker.application.requests_total",
        unit: "{request}");
    private static readonly Histogram<double> RequestDuration = Meter.CreateHistogram<double>(
        "dndtracker.application.request.duration",
        unit: "s");

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

        KeyValuePair<string, object?>[] tags =
        [
            new("mediatR.request.name", requestName),
            new("mediatR.request.type", handlerType),
            new("mediatR.request.fullName", typeof(TRequest).FullName ?? requestName)
        ];

        Stopwatch stopwatch = Stopwatch.StartNew();

        using Activity? activity = ActivitySource.StartActivity(
            requestName,
            ActivityKind.Internal);

        activity?.SetTag("mediatR.request.name", requestName);
        activity?.SetTag("mediatR.request.type", handlerType);
        activity?.SetTag("mediatR.request.fullName", typeof(TRequest).FullName);

        try
        {
            TResponse response = await next();
            RequestDuration.Record(stopwatch.Elapsed.TotalSeconds, tags);
            Requests.Add(1, tags);
            activity?.SetStatus(ActivityStatusCode.Ok);
            return response;
        }
        catch (Exception ex)
        {
            RequestDuration.Record(stopwatch.Elapsed.TotalSeconds, tags);
            Requests.Add(1, tags);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("exception.type", ex.GetType().Name);
            activity?.SetTag("exception.message", ex.Message);
            throw;
        }
    }
}
