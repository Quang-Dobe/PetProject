using PetProject.IdentityServer.Domain.Entities;

namespace PetProject.IdentityServer.Domain.Repositories
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
    {
        void Add(string refreshToken, string securityAlgorithm, User? user, ClientApplication? client);

        Task DeleteAsync(RefreshToken refreshToken);

        bool CompareClientSecret(string hashedClientSecret, string currentClientSecret, string securityAlgorithm);
    }
}