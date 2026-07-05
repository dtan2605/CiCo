using MediatR;

namespace cico.Application.Features.Users.Commands.LockUser
{
    public record LockUserCommand(
        Guid UserId,
        DateTime? LockoutEnd
) : IRequest;
}