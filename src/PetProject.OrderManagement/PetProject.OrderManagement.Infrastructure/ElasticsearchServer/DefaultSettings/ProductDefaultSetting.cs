using Nest;
using PetProject.OrderManagement.Domain.Entities;

namespace PetProject.OrderManagement.Infrastructure.ElasticsearchServer.DefaultSetting
{
    public class ProductDefaultSetting : BaseDefaultSetting
    {
        public ProductDefaultSetting()
        { }

        public override void SetDefaultMapping(ConnectionSettings settings)
        {
            base.SetDefaultMapping(settings);
        }

        public override void SetIndex(IElasticClient client, string indexName)
        {
            base.SetIndex(client, indexName);
            client.Indices.Create(indexName, index => index.Map<Product>(x => x.AutoMap().Dynamic()));
        }
    }
}
