using MediatR;
using HR.Application.Features.LeaveRequests.Commands.Queries.GetLeaveRequests;
using HR.Application.Interfaces;

public class GetLeaveRequestsHandler : IRequestHandler<GetLeaveRequestsQuery, IEnumerable<LeaveRequestDto>>
{
    private readonly IUnitOfWork _uow;

    public GetLeaveRequestsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<LeaveRequestDto>> Handle(GetLeaveRequestsQuery request, CancellationToken cancellationToken)
    {
        var requests = await _uow.LeaveRequests.GetByEmployeeIdAsync(request.EmployeeId);

        return requests.Select(r => new LeaveRequestDto(
            r.Id, r.LeaveType, r.StartDate, r.EndDate,
            r.TotalDays, r.Reason, r.Status, r.RequestedOn));
    }
}