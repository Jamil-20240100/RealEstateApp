using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class SalesTypeEntityConfiguration : IEntityTypeConfiguration<SalesType>
    {
        public void Configure(EntityTypeBuilder<SalesType> builder)
        {
            builder.ToTable("SalesTypes");
            builder.HasKey(st => st.Id);

            builder.Property(st => st.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(st => st.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasMany(st => st.Properties)
                   .WithOne(p => p.SalesType)
                   .HasForeignKey("SalesTypeId");
        }
    }
}