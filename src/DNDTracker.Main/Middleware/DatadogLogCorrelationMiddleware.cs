using Datadog.Trace;
using Serilog.Context;

namespace DNDTracker.Main.Middleware;

/// <summary>
/// Normalizes the request query string (keys only, sorted alphabetically, values
/// stripped) so that requests differing only by parameter order or value are treated
/// as equivalent, and attaches it both to the active Datadog span (as a tag) and to the
/// Serilog log context (so it shows up on every log line for the request).
/// Trace/span log correlation itself is handled automatically by the native Datadog
/// tracer via Serilog's <c>LogContext</c> enricher (<c>Enrich.FromLogContext()</c> in
/// Program.cs) - no manual dd.trace_id/dd.span_id injection is needed here.
/// </summary>
public class DatadogLogCorrelationMiddleware
{
    private readonly RequestDelegate _next;

    public DatadogLogCorrelationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string normalizedQueryString = NormalizeQueryString(context.Request.Query);

        ISpan? activeSpan = Tracer.Instance.ActiveScope?.Span;
        activeSpan?.SetTag("normalizedquerystring", normalizedQueryString);

        using (LogContext.PushProperty("normalizedquerystring", normalizedQueryString))
        {
            await _next(context);
        }
    }

    /// <summary>
    /// Builds a normalized representation of the query string containing only the
    /// parameter keys (no values), sorted alphabetically, so that requests differing
    /// only by parameter order or value are treated as equivalent.
    /// Example: "?status=active&amp;value=curseOfStrahd" becomes "?status&amp;value".
    /// </summary>
    private static string NormalizeQueryString(IQueryCollection query)
    {
        if (query.Count == 0)
        {
            return string.Empty;
        }

        string[] sortedKeys = query.Keys
            .Where(key => !string.IsNullOrEmpty(key))
            .OrderBy(key => key, StringComparer.Ordinal)
            .ToArray();

        return sortedKeys.Length == 0
            ? string.Empty
            : "?" + string.Join('&', sortedKeys);
    }
}

