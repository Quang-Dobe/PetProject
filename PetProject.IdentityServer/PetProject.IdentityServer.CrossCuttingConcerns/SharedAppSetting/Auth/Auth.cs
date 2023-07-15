using Microsoft.Extensions.Configuration;

namespace PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting
{
    public class Auth
    {
        public Auth(IConfiguration configuration) 
        {
            Jwt = new Jwt(configuration);
            AccessTokenLifetime = new AccessTokenLifetime(configuration);
            RefreshTokenLifetime = new RefreshTokenLifetime(configuration);
        }

        public Jwt Jwt { get; set; }

        public AccessTokenLifetime AccessTokenLifetime { get; set; }

        public RefreshTokenLifetime RefreshTokenLifetime { get; set; }
    }
}
