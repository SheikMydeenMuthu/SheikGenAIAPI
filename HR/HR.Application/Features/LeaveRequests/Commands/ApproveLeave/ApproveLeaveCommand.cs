using MediatR;
namespace HR.Application.Features.LeaveRequests.Commands.ApproveLeave;
public record ApproveLeaveCommand(
    Guid LeaveRequestId,
    Guid ManagerId,
    bool IsApproved) : IRequest<bool>;