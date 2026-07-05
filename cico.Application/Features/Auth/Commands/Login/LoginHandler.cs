using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;
using Microsoft.AspNetCore.Identity;
using cico.Domain.Entities;
using cico.Application.Interfaces;
using RefreshTokenEntity = cico.Domain.Entities.RefreshToken;

namespace cico.Application.Features.Auth.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepo;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher<User> _hasher;
    private readonly IRefreshTokenRepository _refreshRepo;

    public LoginHandler(
        IUserRepository userRepo,
        IJwtService jwtService,
        IPasswordHasher<User> hasher,
        IRefreshTokenRepository refreshRepo)
    {
        _userRepo = userRepo;
        _jwtService = jwtService;
        _hasher = hasher;
        _refreshRepo = refreshRepo;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepo.GetByEmailWithPermissionsAsync(request.Email);

        if (user == null)
            throw new DomainException("Invalid credentials");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
            throw new DomainException("Invalid credentials");
        
        var permissions =user.Role.RolePermissions.Select(x => x.Permission.Name).ToList();
        var accessToken = _jwtService.GenerateToken(user.Id, user.Username, user.Role.Name, permissions);
        var refreshToken = _jwtService.GenerateRefreshToken();

        await _refreshRepo.AddAsync(new RefreshTokenEntity
        {
            Token = refreshToken,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            IsRevoked = false
        });

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id,
            Role = user.Role.Name,
            Email = user.Email
        };
    }
}