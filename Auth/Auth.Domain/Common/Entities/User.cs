using Auth.Domain.Common;

namespace Auth.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public bool IsActive { get; set; } = true;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = default!;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}