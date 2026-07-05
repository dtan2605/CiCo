using cico.Domain.Entities;
using MediatR;

namespace cico.Application.Features.Users.Commands.UpdateUser
{
public record UpdateUserCommand(
    Guid Id,
    string Email,
    string Username,
    bool IsActive,
    Guid RoleId
) : IRequest;
}