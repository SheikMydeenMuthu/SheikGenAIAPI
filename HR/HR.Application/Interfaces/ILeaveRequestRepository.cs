namespace HR.Application.Interfaces;
using HR.Domain.Entities;
public interface ILeaveRequestRepository
{
    Task<LeaveRequest?> GetByIdAsync(Guid id);
    Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(Guid employeeId);
    Task<IEnumerable<LeaveRequest>> GetPendingByManagerIdAsync(Guid managerId);
    Task AddAsync(LeaveRequest request);
    Task UpdateAsync(LeaveRequest request);
}