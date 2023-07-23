using PetProject.OrderManagement.Domain.Entities.BaseEntity;

namespace PetProject.OrderManagement.Domain.Entities
{
    public class Port : AggregateEntity<Guid>
    {
        public string IdCode { get; set; }

        public string PortName { get; set; }

        public string Address { get; set; }

        public string Country { get; set; }

        public IEnumerable<Organisation> Organisations { get; set; }
    }
}
