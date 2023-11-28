using PetProject.StoreManagement.Domain.Entities.BaseEntity;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetProject.StoreManagement.Domain.Entities
{
    public class Storage : BaseEntity<Guid>
    {
        public string IdCode { get; set; }

        public string StorageName { get; set; }

        public double Length { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public Guid PortId { get; set; }

        public Guid OrganisationId { get; set; }

        [ForeignKey(nameof(PortId))]
        public Port Port { get; set; }

        [ForeignKey(nameof(OrganisationId))]
        public Organisation Organisation { get; set; }
    }
}
