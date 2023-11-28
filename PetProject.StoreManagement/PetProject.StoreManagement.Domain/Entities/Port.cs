using PetProject.StoreManagement.Domain.Entities.BaseEntity;

namespace PetProject.StoreManagement.Domain.Entities
{
    public class Port : BaseEntity<Guid>
    {
        public string IdCode { get; set; }

        public string PortName { get; set; }

        public string Address { get; set; }

        public string Country { get; set; }

        public IEnumerable<Organisation> Organisations { get; set; }
    }
}
