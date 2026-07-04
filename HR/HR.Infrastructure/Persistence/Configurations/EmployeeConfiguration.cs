using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace HR.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(200);
        builder.HasIndex(e => e.Email).IsUnique();
        builder.Property(e => e.Department).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Designation).IsRequired().HasMaxLength(100);

        builder.HasOne(e => e.ReportingManager)
               .WithMany()
               .HasForeignKey(e => e.ReportingManagerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}