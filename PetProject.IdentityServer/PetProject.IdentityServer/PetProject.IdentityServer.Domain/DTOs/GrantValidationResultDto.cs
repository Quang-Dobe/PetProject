using System.Text.Json.Serialization;

namespace PetProject.IdentityServer.Domain.DTOs
{
    public class GrantValidationResultDto
    {
        [JsonPropertyName("grant_type")]
        public string? GrantType { get; set; }
    }
}