using Auth.Application.DTOs;
using MediatR;

namespace Auth.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<LoginResponseDto>;