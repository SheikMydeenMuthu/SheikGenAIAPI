using HR.Application.Interfaces;
using HR.Domain.Entities;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;
public class EmployeeRepository : IEmployeeRepository
{
    private readonly HRDbContext _context;

    public EmployeeRepository(HRDbContext context) => _context = context;

    public async Task<Employee?> GetByIdAsync(Guid id) =>
        await _context.Employees.Include(e => e.ReportingManager).FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Employee?> GetByEmailAsync(string email) =>
        await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);

    public async Task<IEnumerable<Employee>> GetAllAsync() =>
        await _context.Employees.Include(e => e.ReportingManager).ToListAsync();

    public async Task AddAsync(Employee employee) =>
        await _context.Employees.AddAsync(employee);

    public Task UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        return Task.CompletedTask;
    }
}