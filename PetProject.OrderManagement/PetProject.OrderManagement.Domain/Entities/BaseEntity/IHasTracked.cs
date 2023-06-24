using System.ComponentModel.DataAnnotations;

namespace PetProject.OrderManagement.Domain.Entities.BaseEntity
{
    public interface IHasTracked
    {
        [Timestamp]
        byte[] Version { get; set; }
    }
}