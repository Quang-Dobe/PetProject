using PetProject.IdentityServer.CrossCuttingConcerns.OS;
using PetProject.IdentityServer.Domain.Entities;
using PetProject.IdentityServer.Domain.Repositories;

namespace PetProject.IdentityServer.Persistence.Repositories
{
    public class SmsRepository : BaseRepository<Sms>, ISmsRepository
    {
        public SmsRepository(IdentityDbContext dbContext, IDateTimeProvider dateTimeProvider) : base(dbContext, dateTimeProvider)
        { }
    }
}
