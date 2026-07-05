using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;
using cico.Application.Features.Auth.Commands.Login;
using cico.Domain.Entities;
using cico.Application.Interfaces;
using RefreshTokenEntity = cico.Domain.Entities.RefreshToken;

namespace cico.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly IJwtService _jwt;
    private readonly IRefreshTokenRepository _repo;

    public RefreshTokenHandler(
        IJwtService jwt, 
        IRefreshTokenRepository repo)
    {
        _jwt = jwt;
        _repo = repo;
    }

    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var stored = await _repo.GetByTokenAsync(request.RefreshToken, includeUser: true);

        if (stored == null || stored.IsUsed || stored.IsRevoked)
            throw new DomainException("Invalid refresh token");

        stored.IsUsed = true;

        await _repo.UpdateAsync(stored);
        var permissions = stored.User.Role
            .RolePermissions
            .Select(x => x.Permission.Name)
            .ToList();
        var newAccessToken = _jwt.GenerateToken(
            stored.UserId, 
            stored.User.Username, 
            stored.User.Role.Name,
            permissions);
        var newRefreshToken = _jwt.GenerateRefreshToken();

        await _repo.AddAsync(new RefreshTokenEntity
        {
            Token = newRefreshToken,
            UserId = stored.UserId,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        return new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}