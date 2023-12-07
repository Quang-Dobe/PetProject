using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetProject.IdentityServer.Domain.Entities;

namespace PetProject.IdentityServer.Persistence.EntityConfigurations
{
    public class ClientApplicationConfiguration : IEntityTypeConfiguration<ClientApplication>
    {
        public void Configure(EntityTypeBuilder<ClientApplication> builder)
        {
            builder.ToTable(nameof(ClientApplication));
            builder.HasKey(x => x.Id);
        }
    }
}
