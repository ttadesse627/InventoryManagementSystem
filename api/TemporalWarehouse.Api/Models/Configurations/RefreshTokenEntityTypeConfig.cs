

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemporalWarehouse.Api.Models.Entities;

namespace TemporalWarehouse.Api.Models.EntityConfigurations;

public class RefreshTokenEntityTypeConfig : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(token => token.Id);

        builder.Property(token => token.Token)
            .HasMaxLength(200);

        builder.HasIndex(token => token.Token)
            .IsUnique();

        builder.HasOne(token => token.User)
            .WithMany()
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
