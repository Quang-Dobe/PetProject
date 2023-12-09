using Nest;
using PetProject.OrderManagement.Domain.Entities;

namespace PetProject.OrderManagement.Infrastructure.ElasticsearchServer.DefaultSetting
{
    public class UserDefaultSetting : BaseDefaultSetting
    {
        public UserDefaultSetting()
        { }

        public override void SetDefaultMapping(ConnectionSettings settings)
        {
            base.SetDefaultMapping(settings);
            settings.DefaultMappingFor<User>(x => x.RoutingProperty(t => t.Organisation));
        }

        public override void SetIndex(IElasticClient client, string indexName)
        {
            base.SetIndex(client, indexName);
            client.Indices.Create(indexName, index => index.Map<User>(x => x.AutoMap().Dynamic()));
        }

    }
}
