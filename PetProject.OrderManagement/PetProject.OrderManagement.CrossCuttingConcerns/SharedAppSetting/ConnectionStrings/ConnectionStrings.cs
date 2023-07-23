using Microsoft.Extensions.Configuration;

namespace PetProject.OrderManagement.CrossCuttingConcerns.SharedAppSetting
{
    public class ConnectionStrings
    {
        private readonly IConfiguration _configuration;

        public ConnectionStrings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Identity => _configuration["ConnectionStrings:Identity"];

        public string OrderManagement => _configuration["ConnectionStrings:OrderManagement"];
    }
}
