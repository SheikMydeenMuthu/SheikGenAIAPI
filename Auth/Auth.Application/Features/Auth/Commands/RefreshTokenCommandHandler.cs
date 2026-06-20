using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using MediatR;

namespace Auth.Application.Features.Auth.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existingToken = await _unitOfWork.Users.GetByRefreshTokenAsync(request.RefreshToken);

        if (existingToken is null || !existingToken.IsActive)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        // Revoke old token (rotation)
        existingToken.IsRevoked = true;

        // Issue new tokens
        var newAccessToken = _jwtTokenService.GenerateAccessToken(existingToken.User);
        var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken
        {
            Token = newRefreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = existingToken.UserId
        };

        await _unitOfWork.Users.AddRefreshTokenAsync(newRefreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new LoginResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }
}