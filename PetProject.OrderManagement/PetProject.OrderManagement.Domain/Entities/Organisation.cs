using PetProject.OrderManagement.Domain.Entities.BaseEntity;

namespace PetProject.OrderManagement.Domain.Entities
{
    public class Organisation : AggregateEntity<Guid>
    {
        public string IdCode { get; set; }

        public string OrganisationName { get; set; }

        public string? Address { get; set; }

        public string Country { get; set; }

        public IEnumerable<Container>? Containers { get; set; }

        public IEnumerable<User>? Users { get; set; }

        public IEnumerable<Port>? Ports { get; set; }
    }
}
