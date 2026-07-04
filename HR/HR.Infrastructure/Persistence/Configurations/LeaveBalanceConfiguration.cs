using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;
public class LeaveBalanceConfiguration : IEntityTypeConfiguration<LeaveBalance>
{
    public void Configure(EntityTypeBuilder<LeaveBalance> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LeaveType).HasConversion<string>();
        builder.HasOne(x => x.Employee)
               .WithMany(e => e.LeaveBalances)
               .HasForeignKey(x => x.EmployeeId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(x => x.RemainingDays); // computed property
    }
}