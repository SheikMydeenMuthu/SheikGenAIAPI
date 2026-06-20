using Auth.Application.Interfaces;
using Auth.Infrastructure.Persistence;

namespace Auth.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AuthDbContext _context;
    public IUserRepository Users { get; }
     public IRoleRepository Roles { get; }

    public UnitOfWork(AuthDbContext context, IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _context = context;
        Users = userRepository;
        Roles = roleRepository;
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}