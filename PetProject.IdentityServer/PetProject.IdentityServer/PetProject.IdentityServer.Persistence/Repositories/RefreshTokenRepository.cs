using Microsoft.IdentityModel.Tokens;
using PetProject.IdentityServer.CrossCuttingConcerns.Extensions;
using PetProject.IdentityServer.CrossCuttingConcerns.OS;
using PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting;
using PetProject.IdentityServer.Domain.Entities;
using PetProject.IdentityServer.Domain.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace PetProject.IdentityServer.Persistence.Repositories
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly AppSettings _appSettings;

        private readonly IUnitOfWork _uow;

        public RefreshTokenRepository(IdentityDbContext dbContext, 
                                      IDateTimeProvider dateTimeProvider, 
                                      AppSettings appSettings,
                                      IUnitOfWork uow) : base(dbContext, dateTimeProvider)
        {
            _appSettings = appSettings;
            _uow = uow;
        }

        public void Add(string refreshToken, string securityAlgorithm, User? user, ClientApplication? client)
        {
            var refreshTokenEntity = new RefreshToken();
            refreshTokenEntity.Key = refreshToken.Split('.')[0];
            refreshTokenEntity.TokenHash = EncriptRefreshToken(refreshToken, securityAlgorithm);

            if (user != null)
            {
                refreshTokenEntity.UserId = user.Id.ToString();
                refreshTokenEntity.ClientId = "";
                refreshTokenEntity.CreatedById = user.Id;
                refreshTokenEntity.Expiration = DateTimeOffset.Now.AddMinutes(_appSettings.Auth.RefreshTokenLifetime.ResourceOwnerCredentials + 10);
            }
            else if (client != null)
            {
                refreshTokenEntity.UserId = "";
                refreshTokenEntity.ClientId = client.Id.ToString();
                refreshTokenEntity.CreatedById = client.CreatedById;
                refreshTokenEntity.Expiration = DateTimeOffset.Now.AddMinutes(_appSettings.Auth.RefreshTokenLifetime.ClientCredentials + 10);
            }

            base.Add(refreshTokenEntity);
            _uow.SaveChanges();
        }

        public async Task DeleteAsync(RefreshToken refreshToken)
        {
            base.Delete(refreshToken);
            await _uow.SaveChangesAsync();
        }

        public bool CompareClientSecret(string hashedClientSecret, string currentClientSecret, string securityAlgorithm)
        {
            var hashedCurrentClientSecret = EncriptRefreshToken(currentClientSecret, securityAlgorithm);

            return hashedClientSecret == hashedCurrentClientSecret;
        }

        private string EncriptRefreshToken(string refreshToken, string securityAlgorithm)
        {
            if (securityAlgorithm.IsEqualIgnoreCase(SecurityAlgorithms.Sha256))
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));

                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        stringBuilder.Append(bytes[i].ToString());
                    }

                    return stringBuilder.ToString();
                }
            }

            return "";
        }
    }
}
