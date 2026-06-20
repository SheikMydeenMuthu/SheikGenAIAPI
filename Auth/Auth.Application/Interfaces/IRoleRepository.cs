using Auth.Domain.Entities;

namespace Auth.Application.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name);
}