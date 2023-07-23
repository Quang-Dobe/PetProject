using PetProject.OrderManagement.CrossCuttingConcerns.OS;
using PetProject.OrderManagement.Domain.Entities;
using PetProject.OrderManagement.Domain.Repositories;

namespace PetProject.OrderManagement.Persistence.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(OrderManagementDbContext dbContext, IDateTimeProvider dateTimeProvider) : base(dbContext, dateTimeProvider) { }

    }
}
