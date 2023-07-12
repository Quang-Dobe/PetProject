using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PetProject.IdentityServer.CrossCuttingConcerns.Extensions;
using PetProject.IdentityServer.CrossCuttingConcerns.OS;
using PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting;
using PetProject.IdentityServer.Persistence.MyIdentity.MyManager;
using PetProject.IdentityServer.Persistence.Extensions;
using PetProject.IdentityServer.Domain.Repositories;
using PetProject.IdentityServer.Domain.Entities;
using PetProject.IdentityServer.Domain.Services;
using PetProject.IdentityServer.Domain.DTOs.Credential.Response;
using PetProject.IdentityServer.Domain.DTOs.Credential.Request;

namespace PetProject.IdentityServer.Application.Services
{
    public class AuthorizationService : BaseService, IAuthorizationService
    {
        private readonly MyUserManager _userManager;

        private readonly AppSettings _appSettings;

        private readonly IClientApplicationRepository _clientApplicationRepository;

        private readonly IRefreshTokenRepository _refreshTokenRepository;

        private readonly IUserRepository _userRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly ILogger<AuthorizationService> _logger;

        private readonly string _securityAlgorithm = SecurityAlgorithms.HmacSha256;

        private static readonly object _Lock = new object();

        private Stopwatch _stopwatch;

        public AuthorizationService(MyUserManager userManager, 
            AppSettings appSettings,
            IClientApplicationRepository clientApplicationRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            IDateTimeProvider dateTimeProvider, 
            ILogger<AuthorizationService> logger)
        {
            _userManager = userManager;
            _appSettings = appSettings;
            _clientApplicationRepository = clientApplicationRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        public async Task<CredentialResultDto<ClientCredentialDto>> GrantClientCredentialAsync(LoginDto request)
        {
            string ipAddress = GetIpAddress();
            string clientId;
            string clientSecret;

            _stopwatch = Stopwatch.StartNew();

            if (!_httpContextAccessor.HttpContext.Request.TryGetBasicCredential(out clientId, out clientSecret))
            {
                clientId = request.ClientId;
                clientSecret = request.ClientSecret;
            }

            if (clientId.IsNullOrEmpty() || clientSecret.IsNullOrEmpty())
            {
                LogTrace(request.UserName, clientId, ipAddress, "[AuthorizationService - GrantClientCredential] Invalid ClientID / ClientSecret");
                throw new UnauthorizedAccessException("Invalid ClientID / ClientSecret");
            }

            var client = _clientApplicationRepository.GetClientApplication(clientId, clientSecret);

            if (client != null)
            {
                var issueAt = _dateTimeProvider.Now;
                var permClaims = new List<Claim>()
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, issueAt.ToString()),
                    new Claim("ClientID", clientId)
                };

                var expires = _dateTimeProvider.Now.AddMinutes(_appSettings.Auth.AccessTokenLifetime.ResourceOwnerCredentials);
                var token = GenerateToken(permClaims, expires, _securityAlgorithm);
                var refreshTokenHead = GenerateRefreshToken();
                var refreshTokenTail = GenerateRefreshToken();
                var refreshToken = string.Format("{0}.{1}", refreshTokenHead, refreshTokenTail);

                lock (_Lock)
                {
                    _refreshTokenRepository.Add(refreshToken, _securityAlgorithm, null, client);
                }

                return new CredentialResultDto<ClientCredentialDto>()
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpireIn = (int)_appSettings.Auth.AccessTokenLifetime.ClientCredentials,
                    TokenType = "Bearer",
                    RefreshToken = refreshToken,
                    Credential = new ClientCredentialDto()
                };
            }

            LogTrace(request.UserName, clientId, ipAddress, "[AuthorizationService - GrantClientCredential] Invalid ClientID / ClientSecret");
            throw new UnauthorizedAccessException("Invalid ClientID / ClientSecret");
        }

        public async Task<CredentialResultDto<ResourceOwnerPasswordCredentialDto>> GrantResourceOwnerPasswordCredentialAsync(LoginDto request)
        {
            var ipAddress = GetIpAddress();

            _stopwatch = Stopwatch.StartNew();

            var user = await _userManager.FindByEmailAsync(request.UserName);

            if (user != null)
            {
                if (await _userManager.IsLockedOutAsync(user))
                {
                    LogTrace(request.UserName, request.ClientId, "", "[AuthorizationService - GrantResourceOwnerPassword] User is locked out");
                    throw new UnauthorizedAccessException("User has exceeded login attempts and account is locked out");
                }

                var result = await _userManager.CheckPasswordAsync(user, request.Password);

                if (result)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    _logger.LogInformation("Credentials validated for user {}", user.UserName);

                    if (!user.Status)
                    {
                        LogTrace(request.UserName, request.ClientId, "", "[AuthorizationService - GrantResourceOwnerPassword] User is inactive");
                        throw new UnauthorizedAccessException("User is not active for login");
                    }
                    else if (user.RequirePasswordChanged)
                    {
                        _logger.LogInformation(string.Format("User {0} has never changed password", user.UserName));
                    }
                    else if (user.PasswordExpiredDate.HasValue && user.PasswordExpiredDate <= _dateTimeProvider.Now)
                    {
                        _logger.LogInformation(string.Format("User {0} has to change password", user.UserName));
                    }
                    else
                    {
                        var issueAt = _dateTimeProvider.Now;
                        var permClaims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier, userId),
                            new Claim(ClaimTypes.Email, user.UserName),
                            new Claim("first_name", user.FirstName ?? " "),
                            new Claim("last_name", user.LastName ?? " "),
                            new Claim("full_name", $"{user.FirstName} {user.LastName}"),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Iat, issueAt.ToString())
                        };
                        var expires = _dateTimeProvider.Now.AddMinutes(_appSettings.Auth.AccessTokenLifetime.ResourceOwnerCredentials);
                        var token = GenerateToken(permClaims, expires, _securityAlgorithm);
                        var refreshTokenHead = GenerateRefreshToken();
                        var refreshTokenTail = GenerateRefreshToken();
                        var refreshToken = string.Format("{0}.{1}", refreshTokenHead, refreshTokenTail);

                        lock(_Lock)
                        {
                            _refreshTokenRepository.Add(refreshToken, _securityAlgorithm, user, null);
                        }

                        await _userManager.ResetAccessFailedCountAsync(user);
                        await _userManager.SetLockoutEnabledAsync(user, false);

                        return new CredentialResultDto<ResourceOwnerPasswordCredentialDto>()
                        {
                            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                            ExpireIn = (int)_appSettings.Auth.AccessTokenLifetime.ResourceOwnerCredentials,
                            TokenType = "Bearer",
                            RefreshToken = refreshToken,
                            Credential = new ResourceOwnerPasswordCredentialDto
                            {
                                UserName = user.UserName,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                UserType = IdentityServer.Enums.UserType.External
                            }
                        };
                    }
                }
                else
                {
                    var accessFailedCount = await _userManager.GetAccessFailedCountAsync(user);
                    if (accessFailedCount == 0)
                    {
                        await _userManager.SetLockoutEnabledAsync(user, false);
                    }
                    else if (accessFailedCount + 1 == 3)
                    {
                        await _userManager.SetLockoutEnabledAsync(user, true);
                    }

                    // IsLockedOut => Set LockedEndDate + 5 minutes && ResetAccessFailedCount = 0 (But still lockedOut)
                    await _userManager.AccessFailedAsync(user);
                    _logger.LogInformation(string.Format("User {0} login failed!", user.UserName));

                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        await _userManager.SendEmailAccountLockout(user.UserName, $"{user.FirstName} {user.LastName}");
                        _logger.LogInformation(string.Format("User {0} has been locked out", user.UserName));
                    }

                    LogTrace(request.UserName, request.ClientId, "", "[AuthorizationService - GrantResourceOwnerPassword] Invalid Password");
                    throw new UnauthorizedAccessException("Invalid Username or Password");
                }
            }

            LogTrace(request.UserName, request.ClientId, ipAddress, "[AuthorizationService - GrantResourceOwnerPassword] Invalid Username");
            throw new UnauthorizedAccessException("Invalid Username or Password");
        }

        public async Task<CredentialResultDto<object>> RefreshTokenAsync(LoginDto request)
        {
            var ipAddress = GetIpAddress();
            var refreshTokenHead = request.RefreshToken.Split('.')[0];
            var refreshToken = _refreshTokenRepository.GetAll().Where(x => x.Key == refreshTokenHead).FirstOrDefault();

            if (refreshToken == null)
            {
                LogTrace(request.UserName, request.ClientId, ipAddress, "[AuthorizationService - RefreshToken] Not Exist RefreshToken");
                throw new HttpRequestException("Not Exist RefreshToken");
            }
            else if (refreshToken.ConsumedTime != null)
            {
                LogTrace(request.UserName, request.ClientId, ipAddress, "[AuthorizationService - RefreshToken] Exist Consumed Time");
                throw new HttpRequestException("Exist Consumed Time");
            }
            else if (refreshToken.Expiration < DateTimeOffset.Now)
            {
                await _refreshTokenRepository.DeleteAsync(refreshToken);

                LogTrace(request.UserName, request.ClientId, ipAddress, "[AuthorizationService - RefreshToken] Expired Refresh Token");
                throw new HttpRequestException("Expired Refresh Token");
            }
            else if (request.RefreshToken.IsNullOrEmpty() ||
                !_refreshTokenRepository.CompareClientSecret(refreshToken.TokenHash, request.RefreshToken, _securityAlgorithm))
            {
                LogTrace(request.UserName, request.ClientId, ipAddress, "[AuthorizationService - RefreshToken] Invalid Refresh Token");
            }
            
            if (!refreshToken.UserId.IsNullOrEmpty())
            {
                var result = await RefreshTokenForResourceOwnerCredentialAsync(request, refreshToken);
                return new CredentialResultDto<object>
                {
                    AccessToken = result.AccessToken,
                    ExpireIn = result.ExpireIn,
                    TokenType = result.TokenType,
                    RefreshToken = result.RefreshToken,
                    Credential = result.Credential
                };
            }
            else if (!refreshToken.ClientId.IsNullOrEmpty())
            {
                var result = await RefreshTokenForClientCredentialAsync(request, refreshToken);
                return new CredentialResultDto<object>
                {
                    AccessToken = result.AccessToken,
                    ExpireIn = result.ExpireIn,
                    TokenType = result.TokenType,
                    RefreshToken = result.RefreshToken,
                    Credential = result.Credential
                };
            }

            LogTrace(request.UserName, request.ClientId, ipAddress, "[AuthorizationService - RefreshToken] Invalid UserID / ClientID");
            throw new UnauthorizedAccessException("Invalid Information for Refresh Token");
        }

        public async Task<CredentialResultDto<ClientCredentialDto>> RefreshTokenForClientCredentialAsync(LoginDto request, RefreshToken refreshToken)
        {
            var ipAddress = GetIpAddress();

            try
            {
                var clientId = refreshToken.ClientId;
                var issueAt = _dateTimeProvider.Now;
                var permClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, clientId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, issueAt.ToString()),
                new Claim("urn:oauth:scope", string.Empty),
                new Claim("client_id", clientId)
            };

                var expires = _dateTimeProvider.Now.AddMinutes(_appSettings.Auth.RefreshTokenLifetime.ClientCredentials);
                var token = GenerateToken(permClaims, expires, _securityAlgorithm);
                var client = _clientApplicationRepository.GetAll().Where(x => x.Id == Guid.Parse(clientId)).FirstOrDefault();
                var refreshTokenHead = GenerateRefreshToken();
                var refreshTokenTail = GenerateRefreshToken();
                var new_RefreshToken = string.Format("{0}.{1}", refreshTokenHead, refreshTokenTail);

                lock (_Lock)
                {
                    _refreshTokenRepository.Add(new_RefreshToken, _securityAlgorithm, null, client);
                }

                return new CredentialResultDto<ClientCredentialDto>()
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpireIn = (int)_appSettings.Auth.AccessTokenLifetime.ClientCredentials,
                    TokenType = "Bearer",
                    RefreshToken = new_RefreshToken,
                    Credential = new ClientCredentialDto()
                };
            }
            catch
            {
                LogTrace(request.UserName, refreshToken.ClientId, ipAddress, "[AuthorizationService - RefreshTokenForClientCredential] Invalid ClientID or Something else!");
                throw new UnauthorizedAccessException("There is something wrong");
            }
        }

        public async Task<CredentialResultDto<ResourceOwnerPasswordCredentialDto>> RefreshTokenForResourceOwnerCredentialAsync(LoginDto request, RefreshToken refreshToken)
        {
            var ipAddress = GetIpAddress();

            try
            {
                var userId = refreshToken.UserId;
                var issueAt = _dateTimeProvider.Now;
                var permClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, issueAt.ToString()),
                new Claim("urn:oauth:scope", string.Empty),
                new Claim("user_id", userId)
            };

                var expires = _dateTimeProvider.Now.AddMinutes(_appSettings.Auth.RefreshTokenLifetime.ClientCredentials);
                var token = GenerateToken(permClaims, expires, _securityAlgorithm);
                var user = _userRepository.GetAll().Where(x => x.Id == Guid.Parse(userId)).FirstOrDefault();
                var refreshTokenHead = GenerateRefreshToken();
                var refreshTokenTail = GenerateRefreshToken();
                var new_RefreshToken = string.Format("{0}.{1}", refreshTokenHead, refreshTokenTail);

                lock (_Lock)
                {
                    _refreshTokenRepository.Add(new_RefreshToken, _securityAlgorithm, user, null);
                }

                await _userManager.ResetAccessFailedCountAsync(user);
                await _userManager.SetLockoutEnabledAsync(user, false);
                return new CredentialResultDto<ResourceOwnerPasswordCredentialDto>()
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpireIn = (int)_appSettings.Auth.AccessTokenLifetime.ClientCredentials,
                    TokenType = "Bearer",
                    RefreshToken = new_RefreshToken,
                    Credential = new ResourceOwnerPasswordCredentialDto()
                    {
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserType = IdentityServer.Enums.UserType.External
                    }
                };
            }
            catch
            {
                LogTrace(request.UserName, refreshToken.ClientId, ipAddress, "[AuthorizationService - RefreshTokenForResourceOwnerPassword] Invalid UserID or Something else!");
                throw new UnauthorizedAccessException("There is something wrong");
            }
        }

        #region Private methods

        private JwtSecurityToken GenerateToken(List<Claim> authClaims, DateTime expires, string securityAlgorithm)
        {
            var symetricKey = new SymmetricSecurityKey(Convert.FromBase64String(_appSettings.Auth.Jwt.SymmetricKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Issuer = _appSettings.Auth.Jwt.Issuer,
                Audience = _appSettings.Auth.Jwt.Audience,
                Expires = expires,
                SigningCredentials = new SigningCredentials(symetricKey, securityAlgorithm)
            };

            return tokenHandler.CreateJwtSecurityToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }

        private string GetIpAddress()
        {
            var remoteIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;

            return remoteIpAddress != null ? remoteIpAddress.ToString() : "";
        }

        private void LogTrace(string? userName, string? clientId, string? ipAddress, string? message)
        {
            _stopwatch.Stop();
            _logger.LogInformation(string.Format(" At {0}. Time spent {1} ", _dateTimeProvider.Now, _stopwatch.Elapsed));
            _logger.LogInformation(string.Format(" UserName: {0} - ClientID: {1} - IpAddress: {2} ", userName, clientId, ipAddress));
            _logger.LogInformation(string.Format(" Message: {0} ", message));
        }
        
        #endregion
    }
}
