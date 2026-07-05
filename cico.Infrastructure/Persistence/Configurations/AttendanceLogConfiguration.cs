using cico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cico.Infrastructure.Persistence.Configurations;

public class AttendanceLogConfiguration
    : IEntityTypeConfiguration<AttendanceLog>
{
    public void Configure(
        EntityTypeBuilder<AttendanceLog> builder)
    {
        builder.ToTable("AttendanceLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ScanTime)
            .IsRequired();

        builder.Property(x => x.Message)
            .HasMaxLength(500);

        builder.Property(x => x.IsSuccess)
            .HasDefaultValue(false);

        builder.Property(x => x.Method)
            .HasConversion<int>();

        builder.HasOne(x => x.Attendance)
            .WithMany(x => x.Logs)
            .HasForeignKey(x => x.AttendanceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Device)
            .WithMany(x => x.Logs)
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ScanTime);

        builder.HasIndex(x => x.DeviceId);
    }
}