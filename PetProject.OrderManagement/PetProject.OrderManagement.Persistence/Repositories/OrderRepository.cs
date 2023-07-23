using PetProject.OrderManagement.CrossCuttingConcerns.OS;
using PetProject.OrderManagement.Domain.Entities;
using PetProject.OrderManagement.Domain.Repositories;

namespace PetProject.OrderManagement.Persistence.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(OrderManagementDbContext dbContext, IDateTimeProvider dateTimeProvider) : base(dbContext, dateTimeProvider) { }
    }
}
