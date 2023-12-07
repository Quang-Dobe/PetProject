using PetProject.StoreManagement.Domain.Entities.BaseEntity;

namespace PetProject.StoreManagement.Domain.Entities
{
    public class Organisation : BaseEntity<Guid>
    {
        public string IdCode { get; set; }

        public string OrganisationName { get; set; }

        public string? Address { get; set; }

        public string Country { get; set; }

        public IEnumerable<User>? Users { get; set; }

        public IEnumerable<Port>? Ports { get; set; }
    }
}
