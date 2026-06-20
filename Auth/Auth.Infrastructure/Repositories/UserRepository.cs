using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id) =>
        await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);

    public async Task<RefreshToken?> GetByRefreshTokenAsync(string token) =>
    await _context.RefreshTokens
    .Include(rt => rt.User).ThenInclude(u => u.Role)
    .FirstOrDefaultAsync(rt => rt.Token == token);
        
    public async Task AddRefreshTokenAsync(RefreshToken refreshToken) =>
    await _context.RefreshTokens.AddAsync(refreshToken);

    public async Task AddAsync(User user) => await _context.Users.AddAsync(user);

    public void Update(User user) => _context.Users.Update(user);
}