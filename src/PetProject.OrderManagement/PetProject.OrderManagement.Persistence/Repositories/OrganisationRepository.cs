using PetProject.OrderManagement.CrossCuttingConcerns.OS;
using PetProject.OrderManagement.Domain.Entities;
using PetProject.OrderManagement.Domain.Repositories;
using PetProject.OrderManagement.Domain.Services.BaseService;
using PetProject.OrderManagement.Domain.ThirdPartyServices.BulkActions;

namespace PetProject.OrderManagement.Persistence.Repositories
{
    public class OrganisationRepository : BaseRepository<Organisation>, IOrganisationRepository
    {
        public OrganisationRepository(OrderManagementDbContext dbContext, IDateTimeProvider dateTimeProvider, IExternalRepoService externalRepoService, IBulkActions bulkActions) : base(dbContext, dateTimeProvider, externalRepoService, bulkActions) { }
    }
}
