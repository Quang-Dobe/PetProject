using Microsoft.Extensions.Configuration;

namespace PetProject.StoreManagement.CrossCuttingConcerns.SharedAppSetting
{
    public class InternalAPI
    {
        private readonly IConfiguration _configuration;

        public InternalAPI(IConfiguration configuration)
        {
            _configuration = configuration;
            UserAPI = new UserAPI(configuration);
        }

        public string BaseAPI => _configuration["InternalAPI:BaseAPI"];

        public string AuthorizationAPI => _configuration["InternalAPI:AuthorizationAPI"];

        public UserAPI UserAPI { get; set; }
    }
}
