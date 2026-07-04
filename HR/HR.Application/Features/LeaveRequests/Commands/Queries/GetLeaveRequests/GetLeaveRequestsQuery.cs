using MediatR;
namespace HR.Application.Features.LeaveRequests.Commands.Queries.GetLeaveRequests;
public record GetLeaveRequestsQuery(Guid EmployeeId) : IRequest<IEnumerable<LeaveRequestDto>>;