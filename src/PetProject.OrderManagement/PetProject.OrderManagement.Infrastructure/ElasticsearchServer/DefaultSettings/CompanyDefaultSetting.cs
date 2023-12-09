using Nest;
using PetProject.OrderManagement.Domain.Entities;

namespace PetProject.OrderManagement.Infrastructure.ElasticsearchServer.DefaultSetting
{
    public class CompanyDefaultSetting : BaseDefaultSetting
    {
        public CompanyDefaultSetting()
        { }

        public override void SetDefaultMapping(ConnectionSettings settings)
        {
            base.SetDefaultMapping(settings);
            settings.DefaultMappingFor<Company>(x => x.RoutingProperty(t => t.Clients));
        }

        public override void SetIndex(IElasticClient client, string indexName)
        {
            base.SetIndex(client, indexName);
            client.Indices.Create(indexName, index => index.Map<Company>(x => x.AutoMap().Dynamic()));
        }
    }
}
