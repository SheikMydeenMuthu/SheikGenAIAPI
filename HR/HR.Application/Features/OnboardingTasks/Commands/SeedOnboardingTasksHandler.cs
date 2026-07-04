using HR.Application.Features.OnboardingTasks.Commands;
using HR.Application.Interfaces;
using HR.Domain.Entities;
using HR.Domain.Enums;
using MediatR;

public class SeedOnboardingTasksHandler : IRequestHandler<SeedOnboardingTasksCommand, bool>
{
    private readonly IUnitOfWork _uow;

    public SeedOnboardingTasksHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<bool> Handle(SeedOnboardingTasksCommand request, CancellationToken cancellationToken)
    {
        var tasks = new List<OnboardingTask>
        {
            new() { Id = Guid.NewGuid(), EmployeeId = request.EmployeeId, TaskName = "Setup Laptop", Description = "Configure dev environment", Status = OnboardingTaskStatus.Pending, OrderIndex = 1 },
            new() { Id = Guid.NewGuid(), EmployeeId = request.EmployeeId, TaskName = "Collect ID Card", Description = "Collect ID card from HR", Status = OnboardingTaskStatus.Pending, OrderIndex = 2 },
            new() { Id = Guid.NewGuid(), EmployeeId = request.EmployeeId, TaskName = "Team Introduction", Description = "Meet the team members", Status = OnboardingTaskStatus.Pending, OrderIndex = 3 },
            new() { Id = Guid.NewGuid(), EmployeeId = request.EmployeeId, TaskName = "Complete HR Forms", Description = "Fill and submit all HR documents", Status = OnboardingTaskStatus.Pending, OrderIndex = 4 }
        };

        await _uow.OnboardingTasks.AddRangeAsync(tasks);
        await _uow.SaveChangesAsync();

        return true;
    }
}