using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetProject.StoreManagement.Domain.Entities;

namespace PetProject.StoreManagement.Persistence.EntityConfigurations
{
    public class OrganisationConfiguration : IEntityTypeConfiguration<Organisation>
    {
        public void Configure(EntityTypeBuilder<Organisation> builder) 
        {
            builder.ToTable(nameof(Organisation));
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.IdCode).IsUnique();
        }
    }
}
