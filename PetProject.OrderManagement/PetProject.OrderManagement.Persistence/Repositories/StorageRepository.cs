using PetProject.OrderManagement.CrossCuttingConcerns.OS;
using PetProject.OrderManagement.Domain.Entities;
using PetProject.OrderManagement.Domain.Repositories;

namespace PetProject.OrderManagement.Persistence.Repositories
{
    public class StorageRepository : BaseRepository<Storage>, IStorageRepository
    {
        public StorageRepository(OrderManagementDbContext dbContext, IDateTimeProvider dateTimeProvider) : base(dbContext, dateTimeProvider) { }

    }
}
