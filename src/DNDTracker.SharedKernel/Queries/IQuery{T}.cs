using MediatR;

namespace DNDTracker.SharedKernel.Queries;

public interface IQuery<out T> : IRequest<T>;