using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetProject.IdentityServer.Domain.Entities;

namespace PetProject.IdentityServer.Persistence.EntityConfigurations
{
    public class EmailConfiguration : IEntityTypeConfiguration<Email>
    {
        public void Configure(EntityTypeBuilder<Email> builder)
        {
            builder.ToTable(nameof(Email));
            builder.HasKey(x => x.Id);
        }
    }
}
