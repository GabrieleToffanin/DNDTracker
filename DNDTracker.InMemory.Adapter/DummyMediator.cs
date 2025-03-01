using MediatR;

namespace DNDTracker.InMemory.Adapter;

public class DummyMediator : IMediator
{
    private readonly Dictionary<Type, Func<IBaseRequest, CancellationToken, object>> _handlers = new();

    public DummyMediator RegisterHandler<TRequest, TResponse>(Func<TRequest, CancellationToken, TResponse> handler) 
        where TRequest : IRequest<TResponse>
    {
        _handlers[typeof(TRequest)] = (request, token) => handler((TRequest)request, token)!;
        return this;
    }
    
    public DummyMediator RegisterHandler<TRequest>(Func<TRequest, CancellationToken, Task> handler) 
        where TRequest : IRequest
    {
        _handlers[typeof(TRequest)] = 
            (request, token) => 
                handler((TRequest)request, token);
        return this;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (_handlers.TryGetValue(request.GetType(), out var handler))
        {
            return (TResponse)handler(request, cancellationToken);
        }
        
        throw new InvalidOperationException($"No handler registered for {request.GetType().Name}");
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = new CancellationToken()) where TRequest : IRequest
    {
        return Task.CompletedTask;
    }

    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        await Send<Unit>(new UnitRequest(request), cancellationToken);
    }

    public async Task<object> Send(object request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("This method is not implemented in the dummy mediator");
    }

    public async IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        yield break;
    }

    public async IAsyncEnumerable<object> CreateStream(object request, CancellationToken cancellationToken = default)
    {
        yield break;
    }

    private class UnitRequest : IRequest<Unit>
    {
        public IRequest InnerRequest { get; }

        public UnitRequest(IRequest innerRequest)
        {
            InnerRequest = innerRequest;
        }
    }

    public Task Publish(object notification, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = new CancellationToken()) where TNotification : INotification
    {
        throw new NotImplementedException();
    }
}