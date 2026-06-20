using MediatR;

namespace Auth.Application.Features.Auth.Commands;

public record RegisterCommand(string FullName, string Email, string Password, string? Role) : IRequest<Guid>;