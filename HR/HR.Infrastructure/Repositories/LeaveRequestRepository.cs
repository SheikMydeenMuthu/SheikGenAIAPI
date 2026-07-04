using HR.Application.Interfaces;
using HR.Domain.Entities;
using HR.Domain.Enums;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly HRDbContext _context;

    public LeaveRequestRepository(HRDbContext context) => _context = context;

    public async Task<LeaveRequest?> GetByIdAsync(Guid id) =>
        await _context.LeaveRequests.Include(x => x.Employee).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(Guid employeeId) =>
        await _context.LeaveRequests.Where(x => x.EmployeeId == employeeId).ToListAsync();

    public async Task<IEnumerable<LeaveRequest>> GetPendingByManagerIdAsync(Guid managerId) =>
        await _context.LeaveRequests
            .Include(x => x.Employee)
            .Where(x => x.Employee.ReportingManagerId == managerId && x.Status == LeaveStatus.Pending)
            .ToListAsync();

    public async Task AddAsync(LeaveRequest request) =>
        await _context.LeaveRequests.AddAsync(request);

    public Task UpdateAsync(LeaveRequest request)
    {
        _context.LeaveRequests.Update(request);
        return Task.CompletedTask;
    }
} 