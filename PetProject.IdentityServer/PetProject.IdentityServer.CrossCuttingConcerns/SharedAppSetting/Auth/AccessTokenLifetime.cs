using Microsoft.Extensions.Configuration;

namespace PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting
{
    public class AccessTokenLifetime
    {
        private readonly IConfiguration _configuration;

        public AccessTokenLifetime(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public double ResourceOwnerCredentials => Convert.ToDouble(_configuration["Auth:AccessTokenLifetime:ResourceOwnerCredentials"]);

        public double ClientCredentials => Convert.ToDouble(_configuration["Auth:AccessTokenLifetime:ClientCredentials"]);
    }
}
