using Microsoft.Extensions.Configuration;

namespace PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting
{
    public class Jwt
    {
        private readonly IConfiguration _configuration;

        public Jwt(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Issuer => _configuration["Auth:Jwt:Issuer"];

        public string Audience => _configuration["Auth:Jwt:Audience"];

        public string SymmetricKey => _configuration["Auth:Jwt:SymmetricKey"];
    }
}
