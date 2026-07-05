using Auth.Domain.Entities;
using Auth.Application.Interfaces;
using MediatR;
using System.Net.Http.Json;

namespace Auth.Application.Features.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientFactory _httpClientFactory;

    public RegisterCommandHandler(IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory)
    {
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        
        var existing = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (existing is not null)
            throw new InvalidOperationException("Email already registered");

        var defaultRole = await _unitOfWork.Roles.GetByNameAsync(request.Role ?? "User")
                          ?? throw new InvalidOperationException("User");

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = defaultRole.Id
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var client = _httpClientFactory.CreateClient("HRClient");
        await client.PostAsJsonAsync("api/v1/employees/register", new
        {
            id = user.Id,
            firstName = request.FullName.Split(' ')[0],
            lastName = request.FullName.Split(' ').Length > 1 ? request.FullName.Split(' ')[1] : "",
            email = request.Email,
            department = "General",
            designation = defaultRole.Name,
            reportingManagerId = (Guid?)null,
            joiningDate = DateTime.UtcNow
        }, cancellationToken);

        return user.Id;
    }
}