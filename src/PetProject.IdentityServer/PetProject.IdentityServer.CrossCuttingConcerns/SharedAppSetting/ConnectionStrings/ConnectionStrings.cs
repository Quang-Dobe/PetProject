using Microsoft.Extensions.Configuration;

namespace PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting
{
    public class ConnectionStrings
    {
        private readonly IConfiguration _configuration;

        public ConnectionStrings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Identity => _configuration["ConnectionStrings:Identity"];
    }
}
