using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Users.Commands.Logout;

public class LogoutHandler
    : IRequestHandler<LogoutCommand>
{
    private readonly IUserRepository _userRepo;
    private readonly IRefreshTokenRepository _tokenRepo;

    public LogoutHandler(
        IUserRepository userRepo,
        IRefreshTokenRepository tokenRepo)
    {
        _userRepo = userRepo;
        _tokenRepo = tokenRepo;
    }

    public async Task Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _userRepo.GetByIdAsync(
                request.UserId);

        if (user == null)
            throw new DomainException("User not found");

        var tokens =
            await _tokenRepo
                .GetByUserIdAsync(
                    request.UserId);

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }

        await _tokenRepo
            .UpdateRangeAsync(tokens);

        /*
            revoke ALL JWT
        */
        user.SecurityStamp =
            Guid.NewGuid().ToString();

        await _userRepo.UpdateAsync(user);
    }
}