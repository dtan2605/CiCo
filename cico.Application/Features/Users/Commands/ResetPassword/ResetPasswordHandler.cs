using MediatR;
using cico.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using cico.Domain.Entities;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Users.Commands.ResetPassword;

public class ResetPasswordHandler
    : IRequestHandler<ResetPasswordCommand>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher<User> _hasher;

    public ResetPasswordHandler(
        IUserRepository repo,
        IPasswordHasher<User> hasher)
    {
        _repo = repo;
        _hasher = hasher;
    }

    public async Task Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _repo
                .GetByPasswordResetTokenAsync(
                    request.Token);

        if (user == null)
            throw new DomainException(
                "Token invalid");

        if (user.PasswordResetExpiredAt
            < DateTime.UtcNow)
        {
            throw new DomainException(
                "Token expired");
        }

        user.PasswordHash =
            _hasher.HashPassword(
                user,
                request.NewPassword);

        user.PasswordResetToken = null;
        user.PasswordResetExpiredAt = null;

        user.SecurityStamp =
            Guid.NewGuid().ToString();

        await _repo.UpdateAsync(
            user);
    }
}