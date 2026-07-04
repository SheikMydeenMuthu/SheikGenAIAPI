using HR.Application.Common.Exceptions;
using HR.Application.Features.OnboardingTasks.Commands;
using HR.Application.Interfaces;
using HR.Domain.Entities;
using HR.Domain.Enums;
using MediatR;

namespace HR.Application.Features.OnboardingTasks.Commands;
public class CompleteOnboardingTaskHandler : IRequestHandler<CompleteOnboardingTaskCommand, bool>
{
    private readonly IUnitOfWork _uow;

    public CompleteOnboardingTaskHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<bool> Handle(CompleteOnboardingTaskCommand request, CancellationToken cancellationToken)
    {
        var tasks = await _uow.OnboardingTasks.GetByEmployeeIdAsync(request.EmployeeId);
        var task = tasks.FirstOrDefault(t => t.Id == request.TaskId)
            ?? throw new NotFoundException(nameof(OnboardingTask), request.TaskId);

        task.Status = OnboardingTaskStatus.Completed;
        task.CompletedOn = DateTime.UtcNow;
        await _uow.OnboardingTasks.UpdateAsync(task);

        var allCompleted = tasks.All(t => t.Id == request.TaskId || t.Status == OnboardingTaskStatus.Completed);
        if (allCompleted)
        {
            var employee = await _uow.Employees.GetByIdAsync(request.EmployeeId)
                ?? throw new NotFoundException(nameof(Employee), request.EmployeeId);
            employee.IsOnboarded = true;
            await _uow.Employees.UpdateAsync(employee);
        }

        await _uow.SaveChangesAsync();
        return true;
    }
}