using PetProject.IdentityServer.CrossCuttingConcerns.OS;
using PetProject.IdentityServer.Domain.Entities.BaseEntity;
using PetProject.IdentityServer.Domain.Repositories;

namespace PetProject.IdentityServer.Persistence.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : BaseEntity<Guid>
    {
        private readonly IdentityDbContext _dbContext;

        private readonly IDateTimeProvider _dateTimeProvider;

        public BaseRepository(IdentityDbContext dbContext, IDateTimeProvider dateTimeProvider)
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

        public void SaveChange()
        {
            _dbContext.SaveChanges();
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.AddAsync(entity, cancellationToken);
        }
    
        public async Task SaveChangeAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
