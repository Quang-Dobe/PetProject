using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using PetProject.IdentityServer.CrossCuttingConcerns.Extensions;
using PetProject.IdentityServer.Domain.Entities;

namespace PetProject.IdentityServer.Persistence.MyIdentity
{
    public class PasswordHasher : IPasswordHasher<User>
    {
        public string HashPassword(User user, string password)
        {
            string hashPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(user.SecurityStamp ?? ""),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8)
            );

            return hashPassword;
        }

        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            if (user.SecurityStamp.IsNullOrEmpty())
            {
                return PasswordVerificationResult.Failed;
            }

            var hashOfpasswordToCheck = HashPassword(user, providedPassword);

            if (String.Compare(user.PasswordHash, hashOfpasswordToCheck) == 0)
            {
                return PasswordVerificationResult.Success;
            }

            return PasswordVerificationResult.Failed;
        }
    }
}
