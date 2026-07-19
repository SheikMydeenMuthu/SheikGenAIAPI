namespace Notification.Application.DTOs;

public class LeaveRequestedEvent
{
    public string EmployeeId { get; set; } = default!;
    public string EmployeeName { get; set; } = default!;
    public string ManagerEmail { get; set; } = default!;
    public string LeaveType { get; set; } = default!;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}