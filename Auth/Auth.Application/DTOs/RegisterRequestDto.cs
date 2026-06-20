namespace Auth.Application.DTOs;

public class RegisterRequestDto
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string? Role { get; set; } = "User";
}