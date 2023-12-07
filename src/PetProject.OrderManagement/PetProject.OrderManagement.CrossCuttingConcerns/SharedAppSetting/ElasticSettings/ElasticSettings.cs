using Microsoft.Extensions.Configuration;

namespace PetProject.OrderManagement.CrossCuttingConcerns.SharedAppSetting
{
    public class ElasticSettings
    {
        private readonly IConfiguration _configuration;

        public ElasticSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string BaseUrl => _configuration["ElasticSettings:BaseUrl"];

        public string UserName => _configuration["ElasticSettings:UserName"];

        public string Password => _configuration["ElasticSettings:Password"];

        public string Certificate => _configuration["ElasticSettings:Certificate"];

        public string DefaultIndex => _configuration["ElasticSettings:DefaultIndex"];
    }
}
