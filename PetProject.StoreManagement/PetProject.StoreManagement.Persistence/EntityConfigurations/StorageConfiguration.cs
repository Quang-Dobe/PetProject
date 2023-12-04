using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetProject.StoreManagement.Domain.Entities;

namespace PetProject.StoreManagement.Persistence.EntityConfigurations
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
