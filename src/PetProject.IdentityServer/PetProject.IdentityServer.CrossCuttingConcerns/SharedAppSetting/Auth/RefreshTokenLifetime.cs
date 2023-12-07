using Microsoft.Extensions.Configuration;

namespace PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting
{
    public class RefreshTokenLifetime
    {
        private readonly IConfiguration _configuration;

        public RefreshTokenLifetime(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public double ResourceOwnerCredentials => Convert.ToDouble(_configuration["Auth:RefreshTokenLifetime:ResourceOwnerCredentials"]);

        public double ClientCredentials => Convert.ToDouble(_configuration["Auth:RefreshTokenLifetime:ClientCredentials"]);
    }
}
