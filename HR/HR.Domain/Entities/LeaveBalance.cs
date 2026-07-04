using HR.Domain.Enums;

namespace HR.Domain.Entities;
public class LeaveBalance
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public LeaveType LeaveType { get; set; }
    public int TotalDays { get; set; }
    public int UsedDays { get; set; }
    public int RemainingDays => TotalDays - UsedDays;
    public int Year { get; set; }
}