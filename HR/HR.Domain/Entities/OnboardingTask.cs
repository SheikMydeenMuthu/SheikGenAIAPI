using HR.Domain.Enums;

namespace HR.Domain.Entities;

public class OnboardingTask
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public string TaskName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public OnboardingTaskStatus Status { get; set; } = OnboardingTaskStatus.Pending;
    public int OrderIndex { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedOn { get; set; }
}