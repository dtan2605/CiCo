using cico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cico.Infrastructure.Persistence.Configurations;

public class ScheduleConfiguration
    : IEntityTypeConfiguration<Schedule>
{
    public void Configure(
        EntityTypeBuilder<Schedule> builder)
    {
        builder.ToTable("Schedules");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Name);

        builder.Property(x => x.LateThresholdMinutes)
            .HasDefaultValue(15);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);
    }
}