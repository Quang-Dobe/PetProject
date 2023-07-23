using PetProject.OrderManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetProject.OrderManagement.Domain.Entities
{
    public class Order
    {
        public string IdCode { get; set; }

        public double Length { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double Weight { get; set; }

        public string? Description { get; set; }

        public string? Note { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public OrderStatus Status { get; set; }

        public DestinationType? FromDestinationType { get; set; }

        public DestinationType? ToDestinationType { get; set; }

        public Guid? FromDestination { get; set; }

        public Guid? ToDesination { get; set; }

        public Guid ProductId { get; set; }

        public Guid ContainerId { get; set; }

        public Guid ClientId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [ForeignKey(nameof(ContainerId))]
        public Container Container { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; }
    }
}
