namespace HR.Application.Interfaces;

using HR.Domain.Entities;
public interface IOnboardingTaskRepository
{
    Task<IEnumerable<OnboardingTask>> GetByEmployeeIdAsync(Guid employeeId);
    Task AddRangeAsync(IEnumerable<OnboardingTask> tasks);
    Task UpdateAsync(OnboardingTask task);
}