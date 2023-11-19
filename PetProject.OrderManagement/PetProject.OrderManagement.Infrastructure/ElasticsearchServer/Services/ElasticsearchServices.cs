namespace PetProject.OrderManagement.Infrastructure.ElasticsearchServer.Services
{
    public class ElasticSearchServices : IElasticSearchServices
    {
        public bool Create<T>(T data)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(T data)
        {
            throw new NotImplementedException();
        }

        public bool Update<T>(T data)
        {
            throw new NotImplementedException();
        }

        public bool Delete<T>(T data)
        {
            throw new NotImplementedException();
        }

        public bool CheckExist<T>(T data)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateAsync<T>(T data, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ReadAsync<T>(T data, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync<T>(T data, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync<T>(T data, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckExistAsync<T>(T data, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
