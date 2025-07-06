using System.Collections.Concurrent;
using System.Diagnostics;

namespace DNDTracker.Main.Middleware;

public class BackpressureMiddleware
{
    private readonly RequestDelegate _next;
    private readonly BackpressureOptions _options;
    private readonly ILogger<BackpressureMiddleware> _logger;
    private readonly ConcurrentDictionary<string, TokenBucket> _buckets = new();

    public BackpressureMiddleware(
        RequestDelegate next,
        BackpressureOptions options,
        ILogger<BackpressureMiddleware> logger)
    {
        _next = next;
        _options = options;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (IsExcludedPath(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var clientId = GetClientIdentifier(context);
        var bucket = _buckets.GetOrAdd(clientId, new TokenBucket(_options));

        if (!bucket.TryConsume())
        {
            await HandleRejection(context, bucket);
            return;
        }
        
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            bucket.RecordResponse(stopwatch.ElapsedMilliseconds);
        }
    }

    private string GetClientIdentifier(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private bool IsExcludedPath(PathString path)
    {
        return _options.ExcludedPaths.Any(excludedPath => 
            path.Value?.Contains(excludedPath, StringComparison.OrdinalIgnoreCase) == true);
    }

    private async Task HandleRejection(HttpContext context, TokenBucket bucket)
    {
        var tokensRemaining = bucket.AvailableTokens;
        var refillTime = bucket.TimeUntilNextRefill;
        
        _logger.LogWarning(
            "Request rejected due to backpressure. Client: {ClientId}, Tokens remaining: {TokensRemaining}, Refill in: {RefillTime}ms",
            GetClientIdentifier(context),
            tokensRemaining,
            refillTime.TotalMilliseconds);

        context.Response.StatusCode = 429;
        context.Response.Headers["Retry-After"] = Math.Max(1, (int)refillTime.TotalSeconds).ToString();
        context.Response.Headers["X-RateLimit-Limit"] = _options.BucketCapacity.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = tokensRemaining.ToString();
        context.Response.Headers["X-RateLimit-Reset"] = DateTimeOffset.UtcNow.Add(refillTime).ToUnixTimeSeconds().ToString();

        await context.Response.WriteAsync("Too Many Requests - Token bucket empty");
    }
}

public class TokenBucket
{
    private readonly Lock _lock = new();
    private readonly int _capacity;
    private readonly TimeSpan _refillInterval;
    private readonly int _tokensPerRefill;
    private double _tokens;
    private DateTime _lastRefill;
    private readonly Queue<long> _responseTimes = new();
    private long _totalRequests;
    private long _rejectedRequests;

    public TokenBucket(BackpressureOptions options)
    {
        _capacity = options.BucketCapacity;
        _refillInterval = options.RefillInterval;
        _tokensPerRefill = options.TokensPerRefill;
        _tokens = _capacity; // Start with full bucket
        _lastRefill = DateTime.UtcNow;
    }

    public int AvailableTokens
    {
        get
        {
            using (_lock.EnterScope())
            {
                RefillTokens();
                return (int)Math.Floor(_tokens);
            }
        }
    }

    public TimeSpan TimeUntilNextRefill
    {
        get
        {
            using (_lock.EnterScope())
            {
                var timeSinceLastRefill = DateTime.UtcNow - _lastRefill;
                var remaining = _refillInterval - timeSinceLastRefill;
                return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
            }
        }
    }

    public double AverageResponseTime
    {
        get
        {
            using (_lock.EnterScope())
            {
                return _responseTimes.Count > 0 ? _responseTimes.Average() : 0;
            }
        }
    }
    
    public long TotalRequests => _totalRequests;
    public long RejectedRequests => _rejectedRequests;
    public double RejectionRate => _totalRequests > 0 ? (double)_rejectedRequests / _totalRequests : 0;

    public bool TryConsume()
    {
        using (_lock.EnterScope())
        {
            Interlocked.Increment(ref _totalRequests);
            RefillTokens();
            
            if (_tokens >= 1.0)
            {
                _tokens -= 1.0;
                return true;
            }
            
            Interlocked.Increment(ref _rejectedRequests);
            return false;
        }
    }

    public void RecordResponse(long responseTimeMs)
    {
        using (_lock.EnterScope())
        {
            _responseTimes.Enqueue(responseTimeMs);
            
            // Keep only last 100 response times for average calculation
            while (_responseTimes.Count > 100)
            {
                _responseTimes.Dequeue();
            }
        }
    }

    private void RefillTokens()
    {
        var now = DateTime.UtcNow;
        var timeSinceLastRefill = now - _lastRefill;
        
        if (timeSinceLastRefill >= _refillInterval)
        {
            var refillCycles = (int)(timeSinceLastRefill.Ticks / _refillInterval.Ticks);
            var tokensToAdd = refillCycles * _tokensPerRefill;
            
            _tokens = Math.Min(_capacity, _tokens + tokensToAdd);
            _lastRefill = _lastRefill.AddTicks(refillCycles * _refillInterval.Ticks);
        }
    }
}

public class BackpressureOptions
{
    // Token Bucket specific settings
    public int BucketCapacity { get; set; } = 10;
    public TimeSpan RefillInterval { get; set; } = TimeSpan.FromSeconds(1);
    public int TokensPerRefill { get; set; } = 2;
    
    // General settings
    public int RetryAfterSeconds { get; set; } = 30;
    public bool EnablePerClientLimiting { get; set; } = true;
    public List<string> ExcludedPaths { get; set; } = new() { "/health", "/metrics" };
}