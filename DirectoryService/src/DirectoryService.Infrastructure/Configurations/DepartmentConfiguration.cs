using DirectoryService.Domain.Enities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static DirectoryService.Domain.Constants.DepartmentConstants;
using static DirectoryService.Domain.Constants.DefaultConstants;


namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasColumnName("id");

        builder.ComplexProperty(d => d.Name, n =>
        {
            n.Property(p => p.Value)
                .HasColumnName("name")
                .HasMaxLength(NAME_MAX_LENGTH)
                .IsRequired();
        });

        builder.ComplexProperty(d => d.Identifier, i =>
        {
            i.Property(p => p.Value)
                .HasColumnName("identifier")
                .HasMaxLength(IDENTIFIER_MAX_LENGTH)
                .IsRequired();
        });

        builder.ComplexProperty(d => d.Path, p =>
        {
            p.Property(x => x.Value)
                .HasColumnName("path")
                .HasMaxLength(HIGH_MAX_LENGTH)
                .IsRequired();
        });

        builder.Property(d => d.Depth)
            .HasColumnName("depth")
            .IsRequired();

        builder.Property(d => d.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Parent)
            .WithMany()
            .HasForeignKey("parent_id");

        builder.HasMany(d => d.DepartmentLocations)
            .WithOne(d => d.Department)
            .HasForeignKey(d => d.DepartmentId);
    }
}