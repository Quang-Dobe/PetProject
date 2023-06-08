using PetProject.IdentityServer.Domain.Entities.BaseEntity;

namespace PetProject.IdentityServer.Domain.Entities
{
    public class ClientApplicationDetail : AbstractEntity<Guid>
    {
        public Guid ClientApplicationId { get; set; }

        public string SecretName { get; set; }

        public string ClientSecretHash { get; set; }

        public DateTime? ValidTo { get; set; }

        public virtual ClientApplication ClientApplication { get; set; }
    }
}
