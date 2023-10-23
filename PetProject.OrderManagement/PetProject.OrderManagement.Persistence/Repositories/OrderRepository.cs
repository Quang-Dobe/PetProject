﻿using PetProject.OrderManagement.CrossCuttingConcerns.OS;
using PetProject.OrderManagement.Domain.Entities;
using PetProject.OrderManagement.Domain.Repositories;
using PetProject.OrderManagement.Domain.Services.BaseService;

namespace PetProject.OrderManagement.Persistence.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(OrderManagementDbContext dbContext, IDateTimeProvider dateTimeProvider, IExternalRepoService externalRepoService) : base(dbContext, dateTimeProvider, externalRepoService) { }
    }
}
