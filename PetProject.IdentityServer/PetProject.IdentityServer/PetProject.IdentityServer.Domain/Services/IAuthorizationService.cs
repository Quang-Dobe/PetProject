using PetProject.IdentityServer.Domain.Entities;
using PetProject.IdentityServer.Domain.DTOs;
using PetProject.IdentityServer.Domain.DTOs.Credential;

namespace PetProject.IdentityServer.Domain.Services
{
    public interface IAuthorizationService : IBaseService
    {
        Task<CredentialResultDto<ClientCredentialDto>> GrantClientCredentialAsync(LoginDto request);

        Task<CredentialResultDto<ResourceOwnerPasswordCredentialDto>> GrantResourceOwnerPasswordCredentialAsync(LoginDto request);

        Task<CredentialResultDto<object>> RefreshTokenAsync(LoginDto request);
        
        Task<CredentialResultDto<ClientCredentialDto>> RefreshTokenForClientCredentialAsync(LoginDto request, RefreshToken refreshToken);

        Task<CredentialResultDto<ResourceOwnerPasswordCredentialDto>> RefreshTokenForResourceOwnerCredentialAsync(LoginDto request, RefreshToken refreshToken);
    }
}
