using Auth.Domain.Common;

namespace Auth.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;

    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;
}