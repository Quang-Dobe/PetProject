namespace PetProject.OrderManagement.Infrastructure.ElasticsearchServer.Services
{
    public interface IElasticSearchServices
    {
        bool Read<T>(T data);

        bool Create<T>(T data);

        bool Update<T>(T data);

        bool Delete<T>(T data);

        bool CheckExist<T>(T data);

        Task<bool> CreateAsync<T>(T data, CancellationToken cancellationToken = default);

        Task<bool> ReadAsync<T>(T data, CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync<T>(T data, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync<T>(T data, CancellationToken cancellationToken = default);

        Task<bool> CheckExistAsync<T>(T data, CancellationToken cancellationToken = default);
    }
}
