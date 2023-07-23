using PetProject.OrderManagement.Domain.Entities.BaseEntity;
using PetProject.OrderManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetProject.OrderManagement.Domain.Entities
{
    public class Container : AggregateEntity<Guid>
    {
        public string IdCode { get; set; }

        public string SealCode { get; set; }

        public double Length { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double TareWeight { get; set; }

        public double NetWeight { get; set; }

        public double GrossWeight { get; set; }

        public ContainerType ContainerType { get; set; }

        public ContainerStatus ContainerStatus { get; set; }

        public Guid OrganisationId { get; set; }

        [ForeignKey(nameof(OrganisationId))]
        public Organisation Organisation { get; set; }
    }
}
