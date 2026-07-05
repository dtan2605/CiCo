using cico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cico.Infrastructure.Persistence.Configurations;

public class DeviceConfiguration
    : IEntityTypeConfiguration<Device>
{
    public void Configure(
        EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DeviceCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Location)
            .HasMaxLength(200);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(50);

        builder.Property(x => x.Username)
            .HasMaxLength(100);

        builder.Property(x => x.Password)
            .HasMaxLength(500);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(x => x.DeviceCode)
            .IsUnique();

        builder.HasIndex(x => x.Name);
    }
}