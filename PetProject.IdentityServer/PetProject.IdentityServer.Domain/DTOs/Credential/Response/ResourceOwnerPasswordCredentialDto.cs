using PetProject.IdentityServer.Enums;

namespace PetProject.IdentityServer.Domain.DTOs.Credential.Response
{
    public class ResourceOwnerPasswordCredentialDto
    {
        public string UserName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public UserType UserType { get; set; }
    }
}