using Nest;
using PetProject.OrderManagement.Domain.Entities;

namespace PetProject.OrderManagement.Infrastructure.ElasticsearchServer.DefaultSetting
{
    public class StorageDefaultMapping : BaseDefaultSetting
    {
        public override void SetDefaultMapping(ConnectionSettings settings)
        {
            base.SetDefaultMapping(settings);
            settings.DefaultMappingFor<Storage>(x => x.RoutingProperty(t => t.Port).RoutingProperty(t => t.Organisation));
        }

        public override void SetIndex(IElasticClient client, string indexName)
        {
            base.SetIndex(client, indexName);
            client.Indices.Create(indexName, index => index.Map<Storage>(x => x.AutoMap().Dynamic()));
        }
    }
}
