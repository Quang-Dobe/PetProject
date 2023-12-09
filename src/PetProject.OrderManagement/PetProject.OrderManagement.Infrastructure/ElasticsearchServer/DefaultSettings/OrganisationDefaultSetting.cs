using Nest;
using PetProject.OrderManagement.Domain.Entities;

namespace PetProject.OrderManagement.Infrastructure.ElasticsearchServer.DefaultSetting
{
    public class OrganisationDefaultSetting : BaseDefaultSetting
    {
        public OrganisationDefaultSetting()
        { }

        public override void SetDefaultMapping(ConnectionSettings settings)
        {
            base.SetDefaultMapping(settings);
            settings.DefaultMappingFor<Organisation>(x => x.RoutingProperty(t => t.Containers).RoutingProperty(t => t.Users).RoutingProperty(t => t.Ports));
        }

        public override void SetIndex(IElasticClient client, string indexName)
        {
            base.SetIndex(client, indexName);
            client.Indices.Create(indexName, index => index.Map<Organisation>(x => x.AutoMap().Dynamic()));
        }
    }
}
