using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Users.Commands.LockUser;

public class LockUserHandler
    : IRequestHandler<LockUserCommand>
{
    private readonly IUserRepository _repo;

    public LockUserHandler(
        IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(
        LockUserCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _repo.GetByIdAsync(
                request.UserId);

        if (user == null)
            throw new DomainException(
                "User not found");

        user.IsLocked = true;

        user.LockoutEnd =request.LockoutEnd;

        await _repo.UpdateAsync(
            user);
    }
}