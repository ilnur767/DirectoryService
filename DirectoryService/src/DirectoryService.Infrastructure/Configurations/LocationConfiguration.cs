using DirectoryService.Domain.Enities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static DirectoryService.Domain.Constants.LocationConstants;

namespace DirectoryService.Infrastructure.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name)
            .HasConversion(d => d.Value, v => LocationName.Create(v).Value)
            .HasColumnName("name")
            .HasMaxLength(NAME_MAX_LENGTH)
            .IsRequired();

        builder.HasIndex(b => b.Name).IsUnique();

        builder.Property(l => l.Address)
            .HasConversion(d => d.Value, v => Address.Create(v).Value)
            .HasColumnName("address")
            .HasMaxLength(ADDRESS_MAX_LENGTH)
            .IsRequired();

        builder.HasIndex(b => b.Address).IsUnique();

        builder.ComplexProperty(l => l.Timezone, p =>
        {
            p.Property(l => l.Value)
                .HasColumnName("time_zone")
                .HasMaxLength(TIMEZONE_MAX_LENGTH)
                .IsRequired();
        });

        builder.Property(l => l.IsActive)
            .HasColumnName("is_active");

        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(l => l.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasMany(d => d.DepartmentLocations)
            .WithOne(d => d.Location)
            .HasForeignKey(d => d.LocationId);
    }
}