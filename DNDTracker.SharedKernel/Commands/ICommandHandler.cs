using MediatR;

namespace DNDTracker.SharedKernel.Commands;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand;