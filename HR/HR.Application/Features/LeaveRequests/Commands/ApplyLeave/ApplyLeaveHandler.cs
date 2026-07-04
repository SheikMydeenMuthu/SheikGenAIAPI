using MediatR;
using HR.Application.Common.Exceptions;
using HR.Application.Interfaces;
using HR.Domain.Entities;
using HR.Domain.Enums;
namespace HR.Application.Features.LeaveRequests.Commands.ApplyLeave;
public class ApplyLeaveHandler : IRequestHandler<ApplyLeaveCommand, Guid>
{
    private readonly IUnitOfWork _uow;

    public ApplyLeaveHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Guid> Handle(ApplyLeaveCommand request, CancellationToken cancellationToken)
    {
        var employee = await _uow.Employees.GetByIdAsync(request.EmployeeId)
            ?? throw new NotFoundException(nameof(Employee), request.EmployeeId);

        var leaveRequest = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            LeaveType = request.LeaveType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Reason = request.Reason,
            Status = LeaveStatus.Pending,
            RequestedOn = DateTime.UtcNow
        };

        await _uow.LeaveRequests.AddAsync(leaveRequest);
        await _uow.SaveChangesAsync();

        return leaveRequest.Id;
    }
}