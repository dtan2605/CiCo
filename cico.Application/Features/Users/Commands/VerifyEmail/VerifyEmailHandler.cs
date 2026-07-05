using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Users.Commands.VerifyEmail;

public class VerifyEmailHandler
    : IRequestHandler<VerifyEmailCommand>
{
    private readonly IUserRepository _repo;

    public VerifyEmailHandler(
        IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(
        VerifyEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _repo
                .GetByEmailVerificationTokenAsync(
                    request.Token);

        if (user == null)
            throw new DomainException(
                "Invalid token");

        user.EmailConfirmed = true;
        user.EmailVerificationToken = null;

        await _repo.UpdateAsync(
            user);
    }
}