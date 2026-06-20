using Auth.Domain.Entities;

namespace Auth.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<RefreshToken?> GetByRefreshTokenAsync(string token);
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    Task AddAsync(User user);
    void Update(User user);
}