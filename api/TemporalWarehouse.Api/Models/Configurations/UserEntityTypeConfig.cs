

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemporalWarehouse.Api.Models.Entities;

namespace TemporalWarehouse.Api.Models.EntityConfigurations;

public class UserEntityTypeConfig : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(user => user.Id);

        builder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(55);
    }
}
