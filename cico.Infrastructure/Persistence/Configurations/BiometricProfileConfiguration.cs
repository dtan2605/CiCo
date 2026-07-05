using cico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cico.Infrastructure.Persistence.Configurations;
public class BiometricProfileConfiguration
    : IEntityTypeConfiguration<BiometricProfile>
{
    public void Configure(
        EntityTypeBuilder<BiometricProfile> builder)
    {
        builder.ToTable("BiometricProfiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Template)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.BiometricProfiles)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
            {
                x.EmployeeId,
                x.Type
            })
            .IsUnique();
    }
}