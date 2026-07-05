using MediatR;
using cico.Domain.Entities;

namespace cico.Application.Features.Users.Commands;
public record CreateUserCommand(
    string Username,
    string Email,
    string Password,
    Guid RoleId
) : IRequest<Guid>;