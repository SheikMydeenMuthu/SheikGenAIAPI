using MediatR;
namespace HR.Application.Features.OnboardingTasks.Commands;
public record CompleteOnboardingTaskCommand(Guid TaskId, Guid EmployeeId) : IRequest<bool>;