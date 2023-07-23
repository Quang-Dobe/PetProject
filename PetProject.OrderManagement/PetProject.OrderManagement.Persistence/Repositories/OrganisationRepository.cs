using PetProject.OrderManagement.CrossCuttingConcerns.OS;
using PetProject.OrderManagement.Domain.Entities;
using PetProject.OrderManagement.Domain.Repositories;

namespace PetProject.OrderManagement.Persistence.Repositories
{
    public class OrganisationRepository : BaseRepository<Organisation>, IOrganisationRepository
    {
        public OrganisationRepository(OrderManagementDbContext dbContext, IDateTimeProvider dateTimeProvider) : base(dbContext, dateTimeProvider) { }

    }
}
