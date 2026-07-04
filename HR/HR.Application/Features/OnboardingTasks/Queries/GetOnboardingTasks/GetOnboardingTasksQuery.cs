using MediatR;
namespace HR.Application.Features.OnboardingTasks.Queries.GetOnboardingTasks;
public record GetOnboardingTasksQuery(Guid EmployeeId) : IRequest<IEnumerable<OnboardingTaskDto>>;