using Auth.Application.DTOs;
using MediatR;

namespace Auth.Application.Features.Auth.Queries;

public record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>;