using PetProject.IdentityServer.Domain.Entities.BaseEntity;

namespace PetProject.IdentityServer.Domain.Entities
{
    public class UserRole : AggregateEntity<Guid>
    {
        public Guid UserId { get; }

        public Guid RoleId { get; }

        public User User { get; set; }

        public Role Role { get; set; }
    }
}