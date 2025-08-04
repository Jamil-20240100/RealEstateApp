using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Size)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.State)
            .HasDefaultValue("Disponible");

        // Relationships
        builder.HasOne(p => p.PropertyType)
            .WithMany(pt => pt.Properties)
            .HasForeignKey(p => p.PropertyTypeId);

        builder.HasOne(p => p.SaleType)
            .WithMany(st => st.Properties)
            .HasForeignKey(p => p.SaleTypeId);

        builder.HasMany(p => p.Images)
            .WithOne(pi => pi.Property)
            .HasForeignKey(pi => pi.PropertyId);

        builder.HasMany(p => p.PropertyImprovements)  // Relación muchos a muchos con la entidad intermedia
            .WithOne(pi => pi.Property)
            .HasForeignKey(pi => pi.PropertyId);

        builder.HasMany(p => p.Offers)
            .WithOne(o => o.Property)
            .HasForeignKey(o => o.PropertyId);

        builder.HasMany(p => p.Messages)
            .WithOne(m => m.Property)
            .HasForeignKey(m => m.PropertyId);
    }
}
