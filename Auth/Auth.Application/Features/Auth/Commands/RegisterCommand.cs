using MediatR;

namespace Auth.Application.Features.Auth.Commands;

public record RegisterCommand(string FullName, string Email, string Password) : IRequest<Guid>;