using Nest;

namespace PetProject.OrderManagement.Infrastructure.ElasticsearchServer.DefaultSetting
{
    public interface IBaseDefaultSetting
    {
        void SetDefaultMapping(ConnectionSettings settings);

        void SetIndex(IElasticClient client, string indexName);
    }
}
