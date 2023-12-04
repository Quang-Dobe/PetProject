using MediatR;

namespace PetProject.StoreManagement.Application.Common.Commands
{
    public interface ICommand
    {
        public interface ICommand<TResult> : IRequest<TResult>
        { }
    }
}
