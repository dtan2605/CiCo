using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Users.Commands.AssignRole;

public class AssignRoleHandler
    : IRequestHandler<AssignRoleCommand>
{
    private readonly IUserRepository _repo;

    public AssignRoleHandler(
        IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(
        AssignRoleCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _repo.GetByIdAsync(
                request.UserId);

        if (user == null)
            throw new DomainException(
                "User not found");

        user.RoleId =
            request.RoleId;

        await _repo.UpdateAsync(
            user);
    }
}