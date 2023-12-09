using Nest;

namespace PetProject.OrderManagement.Infrastructure.ElasticsearchServer.DefaultSetting
{
    public class BaseDefaultSetting : IBaseDefaultSetting
    {
        public BaseDefaultSetting()
        { }

        public virtual void SetDefaultMapping(ConnectionSettings settings)
        {
            settings.DefaultIndex("Id")
                .PrettyJson()
                .ThrowExceptions()
                .DeadTimeout(TimeSpan.FromSeconds(10))
                .MaxDeadTimeout(TimeSpan.FromSeconds(30));
        }

        public virtual void SetIndex(IElasticClient client, string indexName)
        { }
    }
}
