using PetProject.IdentityServer.Domain.Entities.BaseEntity;

namespace PetProject.IdentityServer.Domain.Entities
{
    public class UserClaim : AggregateEntity<Guid>
    {
        public string Type { get; set; }

        public string Value { get; set; }

        public User User { get; set; }
    }
}