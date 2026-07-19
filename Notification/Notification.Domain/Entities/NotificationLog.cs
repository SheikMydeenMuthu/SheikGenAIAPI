namespace Notification.Domain.Entities;

public class NotificationLog
{
    public Guid Id { get; set; }
    public string EmployeeId { get; set; } = default!;
    public string ManagerEmail { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public bool IsSent { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}