using MediatR;
using HR.Application.Common.Exceptions;
using HR.Application.Interfaces;
using HR.Domain.Entities;
using HR.Domain.Enums;
using HR.Application.DTOs;

namespace HR.Application.Features.LeaveRequests.Commands.ApplyLeave;

public class ApplyLeaveHandler : IRequestHandler<ApplyLeaveCommand, Guid>
{
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _eventPublisher;

    public ApplyLeaveHandler(IUnitOfWork uow, IEventPublisher eventPublisher)
    {
        _uow = uow;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(ApplyLeaveCommand request, CancellationToken cancellationToken)
    {
        var employee = await _uow.Employees.GetByIdAsync(request.EmployeeId)
            ?? throw new NotFoundException(nameof(Employee), request.EmployeeId);

        // TODO: confirm Employee has ManagerId property, adjust if named differently
        var manager = await _uow.Employees.GetByIdAsync(employee.ReportingManagerId.Value)
            ?? throw new NotFoundException(nameof(Employee), employee.ReportingManagerId.Value);

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

        await _eventPublisher.PublishAsync("leave-events", new LeaveRequestedEvent
        {
            EmployeeId = employee.Id.ToString(),
            EmployeeName = $"{employee.FirstName} {employee.LastName}",
            ManagerEmail = manager.Email,
            LeaveType = leaveRequest.LeaveType.ToString(),
            FromDate = leaveRequest.StartDate,
            ToDate = leaveRequest.EndDate
        }, cancellationToken);

        return leaveRequest.Id;
    }
}