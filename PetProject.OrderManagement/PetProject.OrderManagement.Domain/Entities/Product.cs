﻿using PetProject.OrderManagement.Domain.Entities.BaseEntity;
using PetProject.OrderManagement.Domain.Enums;

namespace PetProject.OrderManagement.Domain.Entities
{
    public class Product : AggregateEntity<Guid>
    {
        public string IdCode { get; set; }

        public string ProductName { get; set; }

        public string? Description { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public DateTime AvailableFrom { get; set; }

        public DateTime? AvailableTo { get; set; }

        public ProductType? ProductType { get; set; }

        public ProductDangerousType? ProductDangerousType { get; set; }
    }
}
