using PetProject.OrderManagement.Domain.Entities.BaseEntity;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetProject.OrderManagement.Domain.Entities
{
    public class Client : AggregateEntity<Guid>
    {
        public string IdCode { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? MiddleName { get; set; }

        public string? DateOfBirth { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public bool IsActive { get; set; }

        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }
    }
}
