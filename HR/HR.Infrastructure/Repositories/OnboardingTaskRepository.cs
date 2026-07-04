using HR.Application.Interfaces;
using HR.Domain.Entities;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
public class OnboardingTaskRepository : IOnboardingTaskRepository
{
    private readonly HRDbContext _context;

    public OnboardingTaskRepository(HRDbContext context) => _context = context;

    public async Task<IEnumerable<OnboardingTask>> GetByEmployeeIdAsync(Guid employeeId) =>
        await _context.OnboardingTasks
            .Where(x => x.EmployeeId == employeeId)
            .OrderBy(x => x.OrderIndex)
             .ToListAsync<OnboardingTask>();

    public async Task AddRangeAsync(IEnumerable<OnboardingTask> tasks) =>
        await _context.OnboardingTasks.AddRangeAsync(tasks);

    public Task UpdateAsync(OnboardingTask task)
    {
        _context.OnboardingTasks.Update(task);
        return Task.CompletedTask;
    }
}