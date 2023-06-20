using PetProject.IdentityServer.Domain.Entities;

namespace PetProject.IdentityServer.Domain.Repositories
{
    public interface IClientApplicationRepository : IBaseRepository<ClientApplication>
    {
        ClientApplication GetClientApplication(string clientId, string clientSecret);
    }
}