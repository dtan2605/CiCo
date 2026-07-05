using MediatR;
using cico.Application.Common.Interfaces.Persistence;
using System;

namespace cico.Application.Features.Users.Commands.ForgotPassword;

public class ForgotPasswordHandler
    : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IUserRepository _repo;

    public ForgotPasswordHandler(
        IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _repo.GetByEmailAsync(
                request.Email);

        if (user == null)
            return;

        user.PasswordResetToken =
            Guid.NewGuid().ToString();

        user.PasswordResetExpiredAt =
            DateTime.UtcNow
                .AddMinutes(15);

        await _repo.UpdateAsync(
            user);

        // TODO:
        // Send Email
    }
}