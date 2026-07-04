namespace HR.Application.Interfaces;

using HR.Domain.Entities;
public interface IUnitOfWork
{
    IEmployeeRepository Employees { get; }
    ILeaveRequestRepository LeaveRequests { get; }
    IOnboardingTaskRepository OnboardingTasks { get; }
    Task<int> SaveChangesAsync();
}