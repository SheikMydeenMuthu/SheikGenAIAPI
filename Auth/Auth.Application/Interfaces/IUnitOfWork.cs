namespace Auth.Application.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    Task<int> SaveChangesAsync();
}