using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

public class FavoritePropertyConfiguration : IEntityTypeConfiguration<FavoriteProperty>
{
    public void Configure(EntityTypeBuilder<FavoriteProperty> builder)
    {
        builder.ToTable("FavoriteProperties");

        builder.HasKey(fp => fp.Id);

        builder.Property(fp => fp.ClientId)
            .IsRequired()
            .HasMaxLength(450);  // Assuming the ClientId is a string (if using Identity)

        builder.Property(fp => fp.PropertyId)
            .IsRequired();

        // Relationships
        builder.HasOne(fp => fp.Property)
            .WithMany()
            .HasForeignKey(fp => fp.PropertyId)
            .OnDelete(DeleteBehavior.Cascade); // Optionally set delete behavior

        // Index to improve lookups by ClientId and PropertyId
        builder.HasIndex(fp => new { fp.ClientId, fp.PropertyId }).IsUnique();
    }
}
