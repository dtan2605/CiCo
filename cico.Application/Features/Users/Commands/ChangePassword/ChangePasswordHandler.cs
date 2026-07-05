using MediatR;
using cico.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using cico.Domain.Entities;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordHandler
    : IRequestHandler<ChangePasswordCommand>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher<User> _hasher;

    public ChangePasswordHandler(
        IUserRepository repo,
        IPasswordHasher<User> hasher)
    {
        _repo = repo;
        _hasher = hasher;
    }

    public async Task Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _repo.GetByIdAsync(
                request.UserId);

        if (user == null)
            throw new DomainException(
                "User not found");

        var result =
            _hasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.OldPassword);

        if (result ==
            PasswordVerificationResult.Failed)
        {
            throw new DomainException(
                "Old password invalid");
        }

        user.PasswordHash =
            _hasher.HashPassword(
                user,
                request.NewPassword);

        user.SecurityStamp =
            Guid.NewGuid().ToString();

        await _repo.UpdateAsync(
            user);
    }
}