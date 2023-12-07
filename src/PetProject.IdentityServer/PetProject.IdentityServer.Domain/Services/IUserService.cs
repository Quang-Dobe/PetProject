using PetProject.IdentityServer.Domain.DTOs.User.Request;

namespace PetProject.IdentityServer.Domain.Services
{
    public interface IUserService : IBaseService
    {
        Task CreateUser(UserDto userInformation);

        Task UpdateUser(UserDto userInformation);

        Task ChangeUserStatus(Guid userId, bool status);
    }
}
