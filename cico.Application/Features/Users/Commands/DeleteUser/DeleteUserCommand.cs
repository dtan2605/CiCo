using MediatR;

namespace cico.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand(
    Guid Id
) : IRequest;