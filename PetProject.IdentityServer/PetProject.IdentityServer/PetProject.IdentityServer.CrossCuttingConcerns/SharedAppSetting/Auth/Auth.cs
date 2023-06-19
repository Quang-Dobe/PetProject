namespace PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting
{
    public class Auth
    {
        public Jwt Jwt { get; }

        public AccessTokenLifetime AccessTokenLifetime { get; }

        public RefreshTokenLifetime RefreshTokenLifetime { get; }
    }
}
