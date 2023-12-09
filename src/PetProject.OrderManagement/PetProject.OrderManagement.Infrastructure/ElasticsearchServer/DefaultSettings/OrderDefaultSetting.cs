using Nest;
using PetProject.OrderManagement.Domain.Entities;

namespace PetProject.OrderManagement.Infrastructure.ElasticsearchServer.DefaultSetting
{
    public class OrderDefaultSetting : BaseDefaultSetting
    {
        public OrderDefaultSetting()
        { }

        public override void SetDefaultMapping(ConnectionSettings settings)
        {
            base.SetDefaultMapping(settings);
            settings.DefaultMappingFor<Order>(x => x.RoutingProperty(t => t.Product).RoutingProperty(t => t.Container).RoutingProperty(t => t.Client));
        }

        public override void SetIndex(IElasticClient client, string indexName)
        {
            base.SetIndex(client, indexName);
            client.Indices.Create(indexName, index => index.Map<Order>(x => x.AutoMap().Dynamic()));
        }
    }
}
