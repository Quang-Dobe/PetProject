using PetProject.IdentityServer.Domain.Entities.BaseEntity;

namespace PetProject.IdentityServer.Domain.Entities
{
    public class RefreshToken : AbstractEntity<Guid>
    {
        public string Key { get; set; }

        public string? UserId { get; set; }

        public string? ClientId { get; set; }

        public DateTimeOffset Expiration { get; set; }

        public DateTimeOffset? ConsumedTime { get; set; }

        public string TokenHash { get; set; }
    }
}
