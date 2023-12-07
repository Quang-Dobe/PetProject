using Nest;
using PetProject.OrderManagement.Domain.Entities.BaseEntity;

namespace PetProject.OrderManagement.Infrastructure.ElasticsearchServer.Services
{
    public class ElasticSearchServices : IElasticSearchServices
    {
        private readonly IElasticClient _client;

        public ElasticSearchServices(IElasticClient client)
        {
            _client = client;
        }

        public bool Create<T>(T data) where T : BaseEntity<Guid>
        {
            var response = _client.Create<T>(new CreateRequest<T>(data));

            return response != null && response.IsValid;
        }

        public IEnumerable<T?> Search<T>(T data, SearchOptions? options) where T : BaseEntity<Guid>
        {
            var optionFields = options?.fields.Select(x => x.FieldName);
            var dataFields = typeof(T).GetFields().Select(x => x.Name).Where(x => optionFields?.Contains(x) == true);

            var searchQuery = new SearchDescriptor<T>();
            foreach (var field in dataFields)
            {
                var value = options.fields.FirstOrDefault(x => x.FieldName == field).FieldValue;
                searchQuery = searchQuery.Query(q => q.Term(t => t.Name(field).Value(value)));
            }

            var result = _client.Search<T>(searchQuery);

            return result.Documents.AsEnumerable();
        }

        public bool Update<T>(T data) where T : BaseEntity<Guid>
        {
            var response = _client.Update<T>(data, x => x.Doc(data));

            return response != null && response.IsValid;
        }

        public bool Delete<T>(T data) where T : BaseEntity<Guid>
        {
            var response = _client.Delete<T>(data);

            return response != null && response.IsValid;
        }

        public bool CheckExist<T>(T data) where T : BaseEntity<Guid>
        {
            var response = _client.DocumentExists<T>(data);

            return response != null && response.IsValid;
        }

        public async Task<bool> CreateAsync<T>(T data, CancellationToken cancellationToken = default) where T : BaseEntity<Guid>
        {
            var response = await _client.CreateAsync<T>(new CreateRequest<T>(data));

            return response != null && response.IsValid;
        }

        public async Task<IEnumerable<T?>> SearchAsync<T>(T data, SearchOptions? options, CancellationToken cancellationToken = default) where T : BaseEntity<Guid>
        {
            var optionFields = options?.fields.Select(x => x.FieldName);
            var dataFields = typeof(T).GetFields().Select(x => x.Name).Where(x => optionFields?.Contains(x) == true);

            var searchQuery = new SearchDescriptor<T>();
            foreach (var field in dataFields)
            {
                var value = options.fields.FirstOrDefault(x => x.FieldName == field).FieldValue;
                searchQuery = searchQuery.Query(q => q.Term(t => t.Name(field).Value(value)));
            }

            var result = await _client.SearchAsync<T>(searchQuery, cancellationToken);

            return result.Documents.AsEnumerable();
        }

        public async Task<bool> UpdateAsync<T>(T data, CancellationToken cancellationToken = default) where T : BaseEntity<Guid>
        {
            var response = await _client.UpdateAsync<T>(data, x => x.Doc(data));

            return response != null && response.IsValid;
        }

        public async Task<bool> DeleteAsync<T>(T data, CancellationToken cancellationToken = default) where T : BaseEntity<Guid>
        {
            var response = await _client.DeleteAsync<T>(data);

            return response != null && response.IsValid;
        }

        public async Task<bool> CheckExistAsync<T>(T data, CancellationToken cancellationToken = default) where T : BaseEntity<Guid>
        {
            var response = await _client.DocumentExistsAsync<T>(data);

            return response != null && response.IsValid;
        }
    }

    public class SearchOptions
    {
        public IEnumerable<SearchField> fields { get; set; }
    }

    public class SearchField
    {
        public string FieldName { get; set; }

        public string FieldValue { get; set; }
    }
}
