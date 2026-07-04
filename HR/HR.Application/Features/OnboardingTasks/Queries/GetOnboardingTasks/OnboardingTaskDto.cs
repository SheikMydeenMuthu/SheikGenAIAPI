using HR.Domain.Enums;
namespace HR.Application.Features.OnboardingTasks.Queries.GetOnboardingTasks;
public record OnboardingTaskDto(
    Guid Id,
    string TaskName,
    string Description,
    OnboardingTaskStatus Status,
    int OrderIndex,
    DateTime? DueDate,
    DateTime? CompletedOn);