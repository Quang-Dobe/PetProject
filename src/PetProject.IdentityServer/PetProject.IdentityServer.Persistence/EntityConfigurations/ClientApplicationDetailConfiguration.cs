using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetProject.IdentityServer.Domain.Entities;

namespace PetProject.IdentityServer.Persistence.EntityConfigurations
{
    public class ClientApplicationDetailConfiguration : IEntityTypeConfiguration<ClientApplicationDetail>
    {
        public void Configure(EntityTypeBuilder<ClientApplicationDetail> builder)
        {
            builder.ToTable(nameof(ClientApplicationDetail));
            builder.HasKey(x => x.Id);
        }
    }
}
