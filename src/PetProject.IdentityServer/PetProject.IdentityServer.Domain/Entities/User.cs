using PetProject.IdentityServer.Domain.Entities.BaseEntity;
using PetProject.IdentityServer.Enums;

namespace PetProject.IdentityServer.Domain.Entities
{
    public class User : AggregateEntity<Guid>
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string PasswordHash { get; set; }

        public bool Status { get; set; }

        public bool RequirePasswordChanged { get; set; }

        public bool LockoutEnable { get; set; }

        public DateTimeOffset? PasswordExpiredDate { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public UserType UserType { get; set; }

        public int AccessFailedCount { get; set; }

        public bool IsElectronicSignatureActive { get; set; }

        public string? ElectronicSignature { get; set; }

        public string? ElectronicSignatureFileName { get; set; }

        public string SecurityStamp { get; set; }

        public IEnumerable<UserClaim>? UserClaims { get; set; }

        public IEnumerable<UserRole>? UserRoles { get; set; }
    }
}