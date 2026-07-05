using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserHandler
    : IRequestHandler<UpdateUserCommand>
{
    private readonly IUserRepository _repo;

    public UpdateUserHandler(
        IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _repo.GetByIdAsync(
                request.Id);

        if (user == null)
            throw new DomainException(
                "User not found");

        user.Username =
            request.Username;

        user.Email =
            request.Email;

        user.RoleId =
            request.RoleId;

        await _repo.UpdateAsync(
            user);
    }
}