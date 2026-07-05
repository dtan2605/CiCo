using MediatR;

namespace cico.Application.Features.Users.Commands.ChangePassword
{
    public record ChangePasswordCommand(
    Guid UserId,
    string OldPassword,
    string NewPassword
) : IRequest;
}