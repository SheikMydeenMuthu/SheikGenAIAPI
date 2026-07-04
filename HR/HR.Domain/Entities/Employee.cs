namespace HR.Domain.Entities;

public class Employee
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public Guid? ReportingManagerId { get; set; }
    public Employee? ReportingManager { get; set; }
    public DateTime JoiningDate { get; set; }
    public bool IsOnboarded { get; set; } = false;

    // Navigation
    public ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<OnboardingTask> OnboardingTasks { get; set; } = new List<OnboardingTask>();
}
