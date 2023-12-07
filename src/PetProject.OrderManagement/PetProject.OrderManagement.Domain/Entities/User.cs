using PetProject.OrderManagement.Domain.Entities.BaseEntity;
using PetProject.OrderManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetProject.OrderManagement.Domain.Entities
{
    public class User : AggregateEntity<Guid>
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

        public UserType UserType { get; set; }

        public DateTime? StartingDate { get; set; }

        public Guid OrganisationId { get; set; }

        [ForeignKey(nameof(OrganisationId))]
        public Organisation Organisation { get; set; }
    }
}
