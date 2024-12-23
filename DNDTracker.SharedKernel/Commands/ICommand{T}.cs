using MediatR;

namespace DNDTracker.SharedKernel.Commands;

public interface ICommand<out T> : IRequest<T>;