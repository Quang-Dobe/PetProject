using PetProject.IdentityServer.Domain.Entities.BaseEntity;

namespace PetProject.IdentityServer.Domain.Entities
{
    public class Role : AggregateEntity<Guid>
    {
        public string Name { get; set; }

        public string? NormalizedName { get; set; }

        public IEnumerable<RoleClaim> RoleClaims { get; set; }
    }
}