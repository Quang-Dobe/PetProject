using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetProject.StoreManagement.Domain.Entities;

namespace PetProject.StoreManagement.Persistence.EntityConfigurations
{
    public class PortConfiguration : IEntityTypeConfiguration<Port>
    {
        public void Configure(EntityTypeBuilder<Port> builder)
        {
            builder.ToTable(nameof(Port));
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.IdCode).IsUnique();
        }
    }
}
