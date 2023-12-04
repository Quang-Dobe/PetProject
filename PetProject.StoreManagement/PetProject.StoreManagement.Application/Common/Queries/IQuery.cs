using MediatR;

namespace PetProject.StoreManagement.Application.Common.Queries
{
    public interface IQuery<TResult> : IRequest<TResult>
    { }
}
