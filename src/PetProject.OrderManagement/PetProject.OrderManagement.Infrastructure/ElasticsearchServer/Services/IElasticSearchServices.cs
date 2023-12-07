using PetProject.OrderManagement.Domain.Entities.BaseEntity;

namespace PetProject.OrderManagement.Infrastructure.ElasticsearchServer.Services
{
    public interface IElasticSearchServices
    {
        IEnumerable<T?> Search<T>(T data, SearchOptions? options) where T : BaseEntity<Guid>;

        bool Create<T>(T data) where T : BaseEntity<Guid>;

        bool Update<T>(T data) where T : BaseEntity<Guid>;

        bool Delete<T>(T data) where T : BaseEntity<Guid>;

        bool CheckExist<T>(T data) where T : BaseEntity<Guid>;

        Task<IEnumerable<T?>> SearchAsync<T>(T data, SearchOptions? options, CancellationToken cancellationToken = default) where T : BaseEntity<Guid>;

        Task<bool> CreateAsync<T>(T data, CancellationToken cancellationToken = default) where T : BaseEntity<Guid>;

        Task<bool> UpdateAsync<T>(T data, CancellationToken cancellationToken = default) where T : BaseEntity<Guid>;

        Task<bool> DeleteAsync<T>(T data, CancellationToken cancellationToken = default) where T : BaseEntity<Guid>;

        Task<bool> CheckExistAsync<T>(T data, CancellationToken cancellationToken = default) where T : BaseEntity<Guid>;
    }
}
