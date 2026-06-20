using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AuthDbContext _context;

    public RoleRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByNameAsync(string name) =>
        await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
}