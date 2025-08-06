using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Common.Enums;

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("Offers");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.Date)
            .IsRequired();

        builder.Property(o => o.Status)
            .HasDefaultValue(OfferStatus.Pendiente)
            .IsRequired();

        // Relationships
        builder.HasOne(o => o.Property)
            .WithMany(p => p.Offers)
            .HasForeignKey(o => o.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(o => o.ClientId)
            .IsRequired();
    }
}
