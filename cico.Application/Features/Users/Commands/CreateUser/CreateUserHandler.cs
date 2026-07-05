using MediatR;
using cico.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using cico.Domain.Entities;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Users.Commands.CreateUser;

public class CreateUserHandler
    : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher<User> _hasher;

    public CreateUserHandler(
        IUserRepository repo,
        IPasswordHasher<User> hasher)
    {
        _repo = repo;
        _hasher = hasher;
    }

    public async Task<Guid> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        if (await _repo.ExistsEmailAsync(request.Email))
            throw new DomainException("Email already exists");

        if (await _repo.ExistsUsernameAsync(request.Username))
            throw new DomainException("Username already exists");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            RoleId = request.RoleId,
            IsActive = true,
            IsDeleted = false,
            IsLocked = false,
            FailedLoginCount = 0,
            EmailConfirmed = false,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        user.PasswordHash =
            _hasher.HashPassword(
                user,
                request.Password);

        await _repo.AddAsync(user);

        return user.Id;
    }
}