using PetProject.StoreManagement.CrossCuttingConcerns.OS;
using PetProject.StoreManagement.Domain.Entities;
using PetProject.StoreManagement.Domain.Repositories;
using PetProject.StoreManagement.Domain.ThirdPartyServices.BulkActions;
using PetProject.StoreManagement.Domain.ThirdPartyServices.ExternalRepoService;

namespace PetProject.StoreManagement.Persistence.Repositories
{
    public class OrganisationRepository : BaseRepository<Organisation>, IOrganisationRepository
    {
        public OrganisationRepository(StoreManagementDbContext dbContext, IDateTimeProvider dateTimeProvider, IExternalRepoService externalRepoService, IBulkActions bulkActions) : base(dbContext, dateTimeProvider, externalRepoService, bulkActions) { }
    }
}
