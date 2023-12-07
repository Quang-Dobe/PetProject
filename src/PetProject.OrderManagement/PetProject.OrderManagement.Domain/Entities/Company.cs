using PetProject.OrderManagement.Domain.Entities.BaseEntity;

namespace PetProject.OrderManagement.Domain.Entities
{
    public class Company : AggregateEntity<Guid>
    {
        public string IdCode { get; set; }

        public string CompanyName { get; set; }

        public string? Address { get; set; }

        public string Country { get; set; }

        public IEnumerable<Client> Clients { get; set; }
    }
}
