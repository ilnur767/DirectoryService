﻿using DirectoryService.Domain.Enitties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("department_positions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DepartmentId)
            .HasColumnName("department_id")
            .IsRequired();

        builder.Property(x => x.PositionId)
            .HasColumnName("position_id")
            .IsRequired();

        builder.HasIndex(x => new { x.DepartmentId, x.PositionId })
            .IsUnique();
    }
}