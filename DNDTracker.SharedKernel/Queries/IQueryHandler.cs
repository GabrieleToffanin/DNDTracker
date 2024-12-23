using MediatR;

namespace DNDTracker.SharedKernel.Queries;

public interface IQueryHandler<in TQuery> : IRequestHandler<TQuery>
    where TQuery : IQuery;