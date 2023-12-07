namespace PetProject.IdentityServer.Domain.Repositories
{
    public interface IBaseRepository<TEntity>
    {
        IQueryable<TEntity> GetAll();

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        void SaveChange();

        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task SaveChangeAsync();
    }
}
