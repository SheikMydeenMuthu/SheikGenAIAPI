using MediatR;
namespace HR.Application.Features.OnboardingTasks.Commands;
public record SeedOnboardingTasksCommand(Guid EmployeeId) : IRequest<bool>;