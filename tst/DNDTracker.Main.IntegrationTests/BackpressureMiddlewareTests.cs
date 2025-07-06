using DNDTracker.Main.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace DNDTracker.Main.IntegrationTests;

public class BackpressureMiddlewareTests
{
    private readonly Mock<ILogger<BackpressureMiddleware>> _loggerMock;
    private readonly BackpressureOptions _options;

    public BackpressureMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<BackpressureMiddleware>>();
        _options = new BackpressureOptions
        {
            BucketCapacity = 3,
            RefillInterval = TimeSpan.FromSeconds(1),
            TokensPerRefill = 1,
            RetryAfterSeconds = 30
        };
    }

    [Fact]
    public async Task InvokeAsync_WithinLimit_ShouldAllowRequest()
    {
        var context = CreateHttpContext();
        var nextCalled = false;
        
        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new BackpressureMiddleware(next, _options, _loggerMock.Object);

        await middleware.InvokeAsync(context);

        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task InvokeAsync_ExceedingBucketCapacity_ShouldRejectRequest()
    {
        var context1 = CreateHttpContext();
        var context2 = CreateHttpContext(); 
        var context3 = CreateHttpContext();
        var context4 = CreateHttpContext();
        var nextCallCount = 0;
        
        RequestDelegate next = _ =>
        {
            nextCallCount++;
            return Task.CompletedTask;
        };

        var middleware = new BackpressureMiddleware(next, _options, _loggerMock.Object);

        // First 3 requests should succeed (bucket capacity = 3)
        await middleware.InvokeAsync(context1);
        await middleware.InvokeAsync(context2);
        await middleware.InvokeAsync(context3);
        
        // 4th request should be rejected (bucket empty)
        await middleware.InvokeAsync(context4);

        nextCallCount.Should().Be(3);
        context4.Response.StatusCode.Should().Be(429);
    }

    [Fact]
    public async Task InvokeAsync_RejectedRequest_ShouldSetHeaders()
    {
        var context1 = CreateHttpContext();
        var context2 = CreateHttpContext(); 
        var context3 = CreateHttpContext();
        var context4 = CreateHttpContext();
        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new BackpressureMiddleware(next, _options, _loggerMock.Object);

        // Exhaust the bucket capacity
        await middleware.InvokeAsync(context1);
        await middleware.InvokeAsync(context2);
        await middleware.InvokeAsync(context3);
        await middleware.InvokeAsync(context4);

        context4.Response.Headers.Should().ContainKey("Retry-After");
        context4.Response.Headers.Should().ContainKey("X-RateLimit-Limit");
        context4.Response.Headers.Should().ContainKey("X-RateLimit-Remaining");
        context4.Response.Headers.Should().ContainKey("X-RateLimit-Reset");
    }

    [Fact]
    public async Task TokenBucket_ShouldRefillOverTime()
    {
        var context1 = CreateHttpContext();
        var context2 = CreateHttpContext();
        var nextCallCount = 0;
        
        RequestDelegate next = _ =>
        {
            nextCallCount++;
            return Task.CompletedTask;
        };

        // Bucket with capacity 1, refills 1 token every 100ms
        var options = new BackpressureOptions
        {
            BucketCapacity = 1,
            RefillInterval = TimeSpan.FromMilliseconds(100),
            TokensPerRefill = 1,
            RetryAfterSeconds = 1
        };

        var middleware = new BackpressureMiddleware(next, options, _loggerMock.Object);

        // First request should succeed
        await middleware.InvokeAsync(context1);
        nextCallCount.Should().Be(1);

        // Second request should fail (bucket empty)
        await middleware.InvokeAsync(context2);
        nextCallCount.Should().Be(1);
        context2.Response.StatusCode.Should().Be(429);

        // Wait for refill
        await Task.Delay(150);

        // Third request should succeed after refill
        var context3 = CreateHttpContext();
        await middleware.InvokeAsync(context3);
        nextCallCount.Should().Be(2);
        context3.Response.StatusCode.Should().Be(200);
    }

    private static HttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/test";
        context.Response.Body = new MemoryStream();
        return context;
    }
}

public class TokenBucketTests
{
    [Fact]
    public void TryConsume_WithAvailableTokens_ReturnsTrue()
    {
        var options = new BackpressureOptions
        {
            BucketCapacity = 5,
            RefillInterval = TimeSpan.FromSeconds(1),
            TokensPerRefill = 1
        };
        var bucket = new TokenBucket(options);
        
        var result = bucket.TryConsume();
        
        result.Should().BeTrue();
        bucket.AvailableTokens.Should().Be(4);
    }

    [Fact]
    public void TryConsume_WithoutAvailableTokens_ReturnsFalse()
    {
        var options = new BackpressureOptions
        {
            BucketCapacity = 2,
            RefillInterval = TimeSpan.FromSeconds(1),
            TokensPerRefill = 1
        };
        var bucket = new TokenBucket(options);
        
        // Consume all tokens
        bucket.TryConsume();
        bucket.TryConsume();
        
        var result = bucket.TryConsume();
        
        result.Should().BeFalse();
        bucket.AvailableTokens.Should().Be(0);
    }

    [Fact]
    public void RecordResponse_ShouldUpdateMetrics()
    {
        var options = new BackpressureOptions { BucketCapacity = 5, RefillInterval = TimeSpan.FromSeconds(1), TokensPerRefill = 1 };
        var bucket = new TokenBucket(options);
        
        bucket.RecordResponse(100);
        bucket.RecordResponse(200);
        
        bucket.AverageResponseTime.Should().Be(150);
    }

    [Fact]
    public void TryConsume_ShouldTrackStatistics()
    {
        var options = new BackpressureOptions { BucketCapacity = 1, RefillInterval = TimeSpan.FromSeconds(1), TokensPerRefill = 1 };
        var bucket = new TokenBucket(options);
        
        bucket.TryConsume(); // Success
        bucket.TryConsume(); // Failure
        
        bucket.TotalRequests.Should().Be(2);
        bucket.RejectedRequests.Should().Be(1);
        bucket.RejectionRate.Should().Be(0.5);
    }

    [Fact]
    public async Task TokenBucket_ShouldRefillTokensOverTime()
    {
        var options = new BackpressureOptions
        {
            BucketCapacity = 2,
            RefillInterval = TimeSpan.FromMilliseconds(100),
            TokensPerRefill = 1
        };
        var bucket = new TokenBucket(options);
        
        // Consume all tokens
        bucket.TryConsume();
        bucket.TryConsume();
        bucket.AvailableTokens.Should().Be(0);
        
        // Wait for refill
        await Task.Delay(150);
        
        bucket.AvailableTokens.Should().BeGreaterThan(0);
        bucket.TryConsume().Should().BeTrue();
    }
}