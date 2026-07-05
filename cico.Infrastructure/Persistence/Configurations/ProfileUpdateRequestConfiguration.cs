using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using cico.Domain.Entities;

namespace cico.Infrastructure.Persistence.Configurations;

public class ProfileUpdateRequestConfiguration
    : IEntityTypeConfiguration<ProfileUpdateRequest>
{
    public void Configure(
        EntityTypeBuilder<ProfileUpdateRequest> builder)
    {
        builder.ToTable("ProfileUpdateRequests");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FullName)
            .HasMaxLength(100);

        builder.Property(e => e.Email)
            .HasMaxLength(100);

        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(e => e.Address)
            .HasMaxLength(200);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(e => e.Employee)
            .WithMany()
            .HasForeignKey(e => e.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
