using MediatR;
using HR.Application.Common.Exceptions;
using HR.Application.Features.LeaveRequests.Commands.ApproveLeave;
using HR.Application.Interfaces;
using HR.Domain.Entities;
using HR.Domain.Enums;

public class ApproveLeaveHandler : IRequestHandler<ApproveLeaveCommand, bool>
{
    private readonly IUnitOfWork _uow;

    public ApproveLeaveHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<bool> Handle(ApproveLeaveCommand request, CancellationToken cancellationToken)
    {
        var leaveRequest = await _uow.LeaveRequests.GetByIdAsync(request.LeaveRequestId)
            ?? throw new NotFoundException(nameof(LeaveRequest), request.LeaveRequestId);

        leaveRequest.Status = request.IsApproved ? LeaveStatus.Approved : LeaveStatus.Rejected;
        leaveRequest.ApprovedByManagerId = request.ManagerId;
        leaveRequest.ActedOn = DateTime.UtcNow;

        await _uow.LeaveRequests.UpdateAsync(leaveRequest);
        await _uow.SaveChangesAsync();

        return true;
    }
}