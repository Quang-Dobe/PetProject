using PetProject.IdentityServer.Domain.Entities.BaseEntity;

namespace PetProject.IdentityServer.Domain.Entities
{
    public class ClientApplication : AbstractEntity<Guid>
    {
        public string ClientName { get; set; }

        public string ClientId { get; set; }

        public bool IsActive { get; set; }

        public Guid? UserId { get; set; } 

        public virtual IEnumerable<ClientApplicationDetail> ClientApplicationDetails { get; set; }
    }
}
