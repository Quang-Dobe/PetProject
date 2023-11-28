namespace PetProject.StoreManagement.Domain.Repositories
{
    public interface IBaseRepository<TEntity>
    {
        IQueryable<TEntity> GetAll();

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        void SaveChanges();

        void BulkInsert(IEnumerable<TEntity> data, IEnumerable<string> columnNames);

        void BulkUpdate(IEnumerable<TEntity> data, IEnumerable<string> columnNames);

        void BulkMerge(IEnumerable<TEntity> data, IEnumerable<string> columnNames);

        void BulkDelete(IEnumerable<TEntity> data, IEnumerable<string> columnNames);

        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
