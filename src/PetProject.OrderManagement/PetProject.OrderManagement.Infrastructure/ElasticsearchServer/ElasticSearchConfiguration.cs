namespace PetProject.OrderManagement.Infrastructure.ElasticsearchServer
{
    public class ElasticSearchConfiguration
    {
        public string BaseUrl { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Certificate { get; set; }

        public string DefaultIndex { get; set; }
    }
}
