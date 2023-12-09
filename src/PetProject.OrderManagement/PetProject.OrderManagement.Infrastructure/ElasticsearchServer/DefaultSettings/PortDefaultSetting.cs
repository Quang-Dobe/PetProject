using Nest;
using PetProject.OrderManagement.Domain.Entities;

namespace PetProject.OrderManagement.Infrastructure.ElasticsearchServer.DefaultSetting
{
    public class PortDefaultSetting : BaseDefaultSetting
    {
        public PortDefaultSetting()
        { }

        public override void SetDefaultMapping(ConnectionSettings settings)
        {
            base.SetDefaultMapping(settings);
            settings.DefaultMappingFor<Port>(x => x.RoutingProperty(t => t.Organisations));
        }

        public override void SetIndex(IElasticClient client, string indexName)
        {
            base.SetIndex(client, indexName);
            client.Indices.Create(indexName, index => index.Map<Port>(x => x.AutoMap().Dynamic()));
        }
    }
}
