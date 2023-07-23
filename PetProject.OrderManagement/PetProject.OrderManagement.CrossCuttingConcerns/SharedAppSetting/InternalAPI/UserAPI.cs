using Microsoft.Extensions.Configuration;

namespace PetProject.OrderManagement.CrossCuttingConcerns.SharedAppSetting
{
    public class UserAPI
    {
        private readonly IConfiguration _configuration;

        public UserAPI(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ChangeStatus => _configuration["InternalAPI:UserAPI:ChangeStatus"];

        public string Register => _configuration["InternalAPI:UserAPI:Register"];

        public string Update => _configuration["InternalAPI:UserAPI:Update"];
    }
}
