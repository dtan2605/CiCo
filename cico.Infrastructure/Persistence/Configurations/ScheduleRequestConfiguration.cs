using cico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cico.Infrastructure.Persistence.Configurations;

public class ScheduleRequestConfiguration : IEntityTypeConfiguration<ScheduleRequest>
{
    public void Configure(EntityTypeBuilder<ScheduleRequest> builder)
    {
        builder.ToTable("ScheduleRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Reason)
            .HasMaxLength(500);

        builder.Property(x => x.AdminNote)
            .HasMaxLength(500);

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CurrentSchedule)
            .WithMany()
            .HasForeignKey(x => x.CurrentScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RequestedSchedule)
            .WithMany()
            .HasForeignKey(x => x.RequestedScheduleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
