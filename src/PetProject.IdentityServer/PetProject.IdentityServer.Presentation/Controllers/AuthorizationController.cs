using Microsoft.AspNetCore.Mvc;
using PetProject.IdentityServer.Domain.Constants;
using PetProject.IdentityServer.Domain.DTOs.Common;
using PetProject.IdentityServer.Domain.DTOs.Credential.Request;
using PetProject.IdentityServer.Domain.DTOs.Credential.Response;
using PetProject.IdentityServer.Domain.Services;

namespace PetProject.IdentityServer.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpPost]
        [Route("Token")]
        public async Task<IActionResult> GetToken()
        {
            var loginInfo = new LoginDto()
            {
                GrantType = Request.Form["grant_type"],
                UserName = Request.Form["username"],
                Password = Request.Form["password"],
                ClientId = Request.Form["clientId"],
                ClientSecret = Request.Form["clientSecret"]
            };

            switch (loginInfo.GrantType)
            {
                case Constants.GrantType.Password:
                    try
                    {
                        return Ok(await _authorizationService.GrantResourceOwnerPasswordCredentialAsync(loginInfo));
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        return BadRequest(new ErrorResultDto<GrantValidationResultDto>
                        {
                            Message = "Invalid User",
                            Error = ex.Message,
                            Data = new GrantValidationResultDto
                            {
                                GrantType = loginInfo.GrantType
                            }
                        });
                    }
                case Constants.GrantType.RefreshToken:
                    try
                    {
                        return Ok(await _authorizationService.RefreshTokenAsync(loginInfo));
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        return BadRequest(new ErrorResultDto<GrantValidationResultDto>
                        {
                            Message = "Invalid User or ClientApplication",
                            Error = ex.Message,
                            Data = new GrantValidationResultDto
                            {
                                GrantType = loginInfo.GrantType
                            }
                        });
                    }
                case Constants.GrantType.ClientCredential:
                    try
                    {
                        return Ok(await _authorizationService.GrantClientCredentialAsync(loginInfo));
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        return BadRequest(new ErrorResultDto<GrantValidationResultDto>
                        {
                            Message = "Invalid ClientApplication",
                            Error = ex.Message,
                            Data = new GrantValidationResultDto
                            {
                                GrantType = loginInfo.GrantType
                            }
                        });
                    }
                default:
                    return Unauthorized(new ErrorResultDto<GrantValidationResultDto>
                    {
                        Message = "Invalid GrantType",
                        Error = "Invalid GrantType",
                        Data = new GrantValidationResultDto
                        {
                            GrantType = loginInfo.GrantType
                        }
                    });
            }
        }
    }
}
