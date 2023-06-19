namespace PetProject.IdentityServer.Domain.DTOs
{
    public class LoginDto
    {
        public string GrantType { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RefreshToken { get; set; }
    }
}
