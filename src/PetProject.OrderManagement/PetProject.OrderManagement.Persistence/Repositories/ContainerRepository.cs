using PetProject.OrderManagement.CrossCuttingConcerns.OS;
using PetProject.OrderManagement.Domain.Entities;
using PetProject.OrderManagement.Domain.Repositories;
using PetProject.OrderManagement.Domain.Services.BaseService;
using PetProject.OrderManagement.Domain.ThirdPartyServices.BulkActions;

namespace PetProject.OrderManagement.Persistence.Repositories
{
    public class ContainerRepository : BaseRepository<Container>, IContainerRepository
    {
        public ContainerRepository(OrderManagementDbContext dbContext, IDateTimeProvider dateTimeProvider, IExternalRepoService externalRepoService, IBulkActions bulkActions) : base(dbContext, dateTimeProvider, externalRepoService, bulkActions) { }
    }
}
