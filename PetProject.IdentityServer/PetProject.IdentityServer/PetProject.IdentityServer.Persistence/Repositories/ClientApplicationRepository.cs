using Microsoft.EntityFrameworkCore;
using PetProject.IdentityServer.CrossCuttingConcerns.OS;
using PetProject.IdentityServer.Domain.Entities;
using PetProject.IdentityServer.Domain.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace PetProject.IdentityServer.Persistence.Repositories
{
    public class ClientApplicationRepository : BaseRepository<ClientApplication>, IClientApplicationRepository
    {
        public ClientApplicationRepository(IdentityDbContext dbContext, IDateTimeProvider dateTimeProvider) : base(dbContext, dateTimeProvider)
        { }

        public ClientApplication GetClientApplication(string clientId, string clientSecret)
        {
            var hashedClientSecret = EncriptClientSecret(clientSecret);
            var clients = GetAll().Include(x => x.ClientApplicationDetails);
            var specificClient = clients.Where(x => x.ClientId == clientId 
                                            && x.ClientApplicationDetails
                                               .Where(y => y.ClientSecretHash == hashedClientSecret)
                                               .Count() != 0)
                                        .FirstOrDefault();

            return specificClient;
        }

        private string EncriptClientSecret(string clientSecret)
        {
            SHA512 sha512 = SHA512.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(clientSecret);
            byte[] hash = sha512.ComputeHash(bytes);

            return BitConverter.ToString(hash).Replace("-", String.Empty);
        }
    }
}
