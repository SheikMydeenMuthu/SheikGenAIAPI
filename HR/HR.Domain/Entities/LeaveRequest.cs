using HR.Domain.Enums;

namespace HR.Domain.Entities;
public class LeaveRequest
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public LeaveType LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays => (EndDate - StartDate).Days + 1;
    public string Reason { get; set; } = string.Empty;
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public Guid? ApprovedByManagerId { get; set; }
    public DateTime RequestedOn { get; set; } = DateTime.UtcNow;
    public DateTime? ActedOn { get; set; }
}