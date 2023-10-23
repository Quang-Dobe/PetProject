using Nest;
using PetProject.OrderManagement.CrossCuttingConcerns.SharedAppSetting;
using PetProject.OrderManagement.Domain.Entities.BaseEntity;
using PetProject.OrderManagement.Domain.Services.BaseService;

namespace PetProject.OrderManagement.Infrastructure.ElasticsearchServer.Services
{
    public class ElasticsearchServices : IExternalRepoService
    {
        private readonly ElasticClient _client;

        public ElasticsearchServices(AppSettings appSettings, ElasticClient client)
        {
            _client = client;
        }

        public Task GenerateData<T>(T data) where T : BaseEntity<Guid>
        {
            return Task.FromResult(_client.IndexDocumentAsync<T>(data));
        }

        public Task DeleteData<T>(T data) where T : BaseEntity<Guid>
        {
            return Task.FromResult(_client.DeleteAsync<T>(data));
        }

        public Task BulkGenerateData<T>(IEnumerable<T> data) where T : BaseEntity<Guid>
        {
            return Task.FromResult(_client.IndexManyAsync<T>(data));
        }

        public Task BulkDeleteData<T>(IEnumerable<T> data) where T: BaseEntity<Guid>
        {
            return Task.FromResult(_client.DeleteManyAsync<T>(data));
        }
    }
}
