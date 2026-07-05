using MediatR;

namespace cico.Application.Features.Users.Commands.AssignRole
{
    public record AssignRoleCommand(
    Guid UserId,
    Guid RoleId
) : IRequest;
}