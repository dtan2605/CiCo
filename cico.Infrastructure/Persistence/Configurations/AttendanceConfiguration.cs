using cico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cico.Infrastructure.Persistence.Configurations;

public class AttendanceConfiguration
    : IEntityTypeConfiguration<Attendance>
{
    public void Configure(
        EntityTypeBuilder<Attendance> builder)
    {
        builder.ToTable("Attendances");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AttendanceDate)
            .IsRequired();

        builder.Property(x => x.LateMinutes)
            .HasDefaultValue(0);

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.Method)
            .HasConversion<int>();

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.Attendances)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.EmployeeSchedule)
            .WithMany()
            .HasForeignKey(x => x.EmployeeScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x =>
            new
            {
                x.EmployeeId,
                x.AttendanceDate,
            })
            .IsUnique();
    }
}