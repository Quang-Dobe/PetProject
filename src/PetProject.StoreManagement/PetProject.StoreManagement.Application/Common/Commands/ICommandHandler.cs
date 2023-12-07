using MediatR;
using static PetProject.StoreManagement.Application.Common.Commands.ICommand;

namespace PetProject.StoreManagement.Application.Common.Commands
{
    public interface ICommandHandler<TCommand, TResult> : IRequestHandler<TCommand, TResult> 
        where TCommand : ICommand<TResult>
    { }
}
