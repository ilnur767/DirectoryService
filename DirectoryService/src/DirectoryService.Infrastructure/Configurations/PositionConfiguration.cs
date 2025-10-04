using DirectoryService.Domain.Enitties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.Property(p => p.Description)
            .HasMaxLength(100)
            .HasColumnName("description");

        builder.Property(l => l.IsActive)
            .HasColumnName("is_active");

        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(l => l.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasMany(p => p.DepartmentPositions)
            .WithOne(p => p.Position)
            .HasForeignKey(p => p.PositionId);
    }
}