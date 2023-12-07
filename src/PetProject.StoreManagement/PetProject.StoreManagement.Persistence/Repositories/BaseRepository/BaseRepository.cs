using PetProject.StoreManagement.CrossCuttingConcerns.OS;
using PetProject.StoreManagement.Domain.Entities.BaseEntity;
using PetProject.StoreManagement.Domain.Repositories;
using PetProject.StoreManagement.Domain.ThirdPartyServices.BulkActions;
using PetProject.StoreManagement.Domain.ThirdPartyServices.ExternalRepoService;

namespace PetProject.StoreManagement.Persistence.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : BaseEntity<Guid>
    {
        private readonly StoreManagementDbContext _dbContext;

        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly IExternalRepoService _externalRepository;

        private readonly IBulkActions _bulkActions;

        public BaseRepository(StoreManagementDbContext dbContext, IDateTimeProvider dateTimeProvider, IExternalRepoService externalRepository, IBulkActions bulkActions)
        {
            _dbContext = dbContext;
            _dateTimeProvider = dateTimeProvider;
            _externalRepository = externalRepository;
            _bulkActions = bulkActions;
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

        public void BulkInsert(IEnumerable<TEntity> data, IEnumerable<string> columnNames)
        {
            _bulkActions.BulkInsert(data, columnNames);
        }

        public void BulkUpdate(IEnumerable<TEntity> data, IEnumerable<string> columnNames)
        {
            _bulkActions.BulkUpdate(data, columnNames);
        }

        public void BulkMerge(IEnumerable<TEntity> data, IEnumerable<string> columnNames)
        {
            _bulkActions.BulkMerge(data, columnNames);
        }

        public void BulkDelete(IEnumerable<TEntity> data, IEnumerable<string> columnNames)
        {
            _bulkActions.BulkDelete(data, columnNames);
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.AddAsync(entity, cancellationToken);
            await _externalRepository.GenerateData(entity);
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            await _externalRepository.DeleteData(entity);
        }
    
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
