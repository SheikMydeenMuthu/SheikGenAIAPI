using MediatR;
using HR.Domain.Enums;
namespace HR.Application.Features.LeaveRequests.Commands.ApplyLeave;
public record ApplyLeaveCommand(
    Guid EmployeeId,
    LeaveType LeaveType,
    DateTime StartDate,
    DateTime EndDate,
    string Reason) : IRequest<Guid>;