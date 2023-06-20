namespace PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting
{
    public class Jwt
    {
        public string Issuer { get; }

        public string Audience { get; }

        public string SymmetricKey { get; }
    }
}
