using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;

public class PropertyImageEntityConfiguration : IEntityTypeConfiguration<PropertyImage>
{
    public void Configure(EntityTypeBuilder<PropertyImage> builder)
    {
        builder.ToTable("PropertyImages");

        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.ImageUrl)
               .IsRequired()
               .HasMaxLength(500);

        builder.HasOne(pi => pi.Property)
               .WithMany(p => p.Images)
               .HasForeignKey(pi => pi.PropertyId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
