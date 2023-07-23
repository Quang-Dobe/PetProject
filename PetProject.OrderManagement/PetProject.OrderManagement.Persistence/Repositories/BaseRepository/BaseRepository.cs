using PetProject.OrderManagement.CrossCuttingConcerns.OS;
using PetProject.OrderManagement.Domain.Entities.BaseEntity;
using PetProject.OrderManagement.Domain.Repositories;

namespace PetProject.OrderManagement.Persistence.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : BaseEntity<Guid>
    {
        private readonly OrderManagementDbContext _dbContext;

        private readonly IDateTimeProvider _dateTimeProvider;

        public BaseRepository(OrderManagementDbContext dbContext, IDateTimeProvider dateTimeProvider)
        {
            _dbContext = dbContext;
            _dateTimeProvider = dateTimeProvider;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
            entity.CreatedDate = _dateTimeProvider.Now;
            _dbContext.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Added;
        }

        public void Update(TEntity entity)
        {
            entity.UpdatedDate = _dateTimeProvider.Now;
            _dbContext.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
        }
    
        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.AddAsync(entity, cancellationToken);
        }
    
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
