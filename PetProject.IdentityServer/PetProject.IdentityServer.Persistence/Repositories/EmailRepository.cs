using PetProject.IdentityServer.CrossCuttingConcerns.OS;
using PetProject.IdentityServer.Domain.Entities;
using PetProject.IdentityServer.Domain.Repositories;

namespace PetProject.IdentityServer.Persistence.Repositories
{
    public class EmailRepository : BaseRepository<Email>, IEmailRepository
    {
        public EmailRepository(IdentityDbContext dbContext, IDateTimeProvider dateTimeProvider) : base(dbContext, dateTimeProvider)
        { }
    }
}
