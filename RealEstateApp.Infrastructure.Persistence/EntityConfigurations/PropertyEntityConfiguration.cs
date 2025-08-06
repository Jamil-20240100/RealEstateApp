using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class PropertyEntityConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.ToTable("Properties");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.AgentId).IsRequired();

            builder.Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(p => p.SizeInMeters).IsRequired();

            builder.Property(p => p.NumberOfRooms).IsRequired();

            builder.Property(p => p.NumberOfBathrooms).IsRequired();

            builder
    .HasMany(p => p.Images)
    .WithOne(pi => pi.Property)
    .HasForeignKey(pi => pi.PropertyId)
    .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(p => p.PropertyType)
                   .WithMany(pt => pt.Properties)
                   .HasForeignKey(p => p.PropertyTypeId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.HasOne(p => p.SalesType)
                   .WithMany(st => st.Properties)
                   .HasForeignKey(p => p.SalesTypeId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder
                .HasMany(p => p.Features)
                .WithMany(f => f.Properties)
                .UsingEntity(j => j.ToTable("PropertyFeatures"));
        }
    }
}
