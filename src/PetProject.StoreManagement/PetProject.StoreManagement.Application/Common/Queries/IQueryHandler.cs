using MediatR;

namespace PetProject.StoreManagement.Application.Common.Queries
{
    public interface IQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, TResult> 
        where TQuery : IQuery<TResult>
    { }
}
