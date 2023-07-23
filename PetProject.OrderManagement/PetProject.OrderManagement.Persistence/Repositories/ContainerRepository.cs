using PetProject.OrderManagement.CrossCuttingConcerns.OS;
using PetProject.OrderManagement.Domain.Entities;
using PetProject.OrderManagement.Domain.Repositories;

namespace PetProject.OrderManagement.Persistence.Repositories
{
    public class ContainerRepository : BaseRepository<Container>, IContainerRepository
    {
        public ContainerRepository(OrderManagementDbContext dbContext, IDateTimeProvider dateTimeProvider) : base(dbContext, dateTimeProvider) { }

    }
}
