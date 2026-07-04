using HR.Domain.Enums;
namespace HR.Application.Features.LeaveRequests.Commands.Queries.GetLeaveRequests;
public record LeaveRequestDto(
    Guid Id,
    LeaveType LeaveType,
    DateTime StartDate,
    DateTime EndDate,
    int TotalDays,
    string Reason,
    LeaveStatus Status,
    DateTime RequestedOn);