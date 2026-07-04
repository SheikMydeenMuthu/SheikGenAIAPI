using MediatR;
using HR.Application.Interfaces;

namespace HR.Application.Features.OnboardingTasks.Queries.GetOnboardingTasks;
public class GetOnboardingTasksHandler : IRequestHandler<GetOnboardingTasksQuery, IEnumerable<OnboardingTaskDto>>
{
    private readonly IUnitOfWork _uow;

    public GetOnboardingTasksHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<OnboardingTaskDto>> Handle(GetOnboardingTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _uow.OnboardingTasks.GetByEmployeeIdAsync(request.EmployeeId);

        return tasks.Select(t => new OnboardingTaskDto(
            t.Id, t.TaskName, t.Description,
            t.Status, t.OrderIndex, t.DueDate, t.CompletedOn));
    }
}