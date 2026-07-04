using HR.Application.Interfaces;
using HR.Infrastructure.Persistence;

namespace HR.Infrastructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly HRDbContext _context;

    public IEmployeeRepository Employees { get; }
    public ILeaveRequestRepository LeaveRequests { get; }
    public IOnboardingTaskRepository OnboardingTasks { get; }

    public UnitOfWork(HRDbContext context,
        IEmployeeRepository employees,
        ILeaveRequestRepository leaveRequests,
        IOnboardingTaskRepository onboardingTasks)
    {
        _context = context;
        Employees = employees;
        LeaveRequests = leaveRequests;
        OnboardingTasks = onboardingTasks;
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}