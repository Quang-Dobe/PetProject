using PetProject.IdentityServer.Domain.Entities.BaseEntity;

namespace PetProject.IdentityServer.Domain.Entities
{
    public class RoleClaim : AggregateEntity<Guid>
    {
        public string Type { get; set; }

        public string Value { get; set; }

        public Role Role { get; set; }
    }
}