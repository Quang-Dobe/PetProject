using System.ComponentModel.DataAnnotations;

namespace PetProject.IdentityServer.Domain.Entities.BaseEntity
{
    public interface IHasTracked
    {
        [Timestamp]
        byte[] Version { get; set; }
    }
}