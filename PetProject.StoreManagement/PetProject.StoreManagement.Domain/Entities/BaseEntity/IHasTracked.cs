using System.ComponentModel.DataAnnotations;

namespace PetProject.StoreManagement.Domain.Entities.BaseEntity
{
    public interface IHasTracked
    {
        [Timestamp]
        byte[] Version { get; set; }

        bool IsSync { get; set; }

        bool RowDeleted { get; set; }
    }
}