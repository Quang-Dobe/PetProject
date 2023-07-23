using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetProject.OrderManagement.Domain.Entities;

namespace PetProject.OrderManagement.Persistence.EntityConfigurations
{
    public class StorageConfiguration : IEntityTypeConfiguration<Storage>
    {
        public void Configure(EntityTypeBuilder<Storage> builder)
        {
            builder.ToTable(nameof(Storage));
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.IdCode).IsUnique();
        }
    }
}
