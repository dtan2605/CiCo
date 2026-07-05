using MediatR;

namespace cico.Application.Features.Users.Commands.Logout
{
    public record LogoutCommand(
        Guid UserId,
        string RefreshToken
    ) : IRequest;
}