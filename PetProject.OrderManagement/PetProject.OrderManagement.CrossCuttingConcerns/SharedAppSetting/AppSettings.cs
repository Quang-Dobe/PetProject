using Microsoft.Extensions.Configuration;

namespace PetProject.OrderManagement.CrossCuttingConcerns.SharedAppSetting
{
    public class AppSettings
    {
        private readonly IConfiguration _configuration;

        public AppSettings(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionStrings = new ConnectionStrings(configuration);
            InternalAPI = new InternalAPI(configuration);
        }

        public string AllowedHosts => _configuration["AllowedHosts"];

        public ConnectionStrings ConnectionStrings { get; set; }

        public InternalAPI InternalAPI { get; set; }
    }
}
