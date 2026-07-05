using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserHandler
    : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _repo;

    public DeleteUserHandler(
        IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _repo.GetByIdAsync(
                request.Id);

        if (user == null)
            throw new DomainException(
                "User not found");

        user.IsDeleted = true;
        user.IsActive = false;

        await _repo.UpdateAsync(
            user);
    }
}