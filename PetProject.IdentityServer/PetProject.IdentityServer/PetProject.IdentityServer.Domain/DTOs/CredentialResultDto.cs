using System.Text.Json.Serialization;

namespace PetProject.IdentityServer.Domain.DTOs
{
    public class CredentialResultDto<TCredentialType>
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expire_in")]
        public int ExpireIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        public TCredentialType Credential { get; set; }
    }
}