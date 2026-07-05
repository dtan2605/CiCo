using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Positions.Commands.DeletePosition;

public class DeletePositionHandler
    : IRequestHandler<DeletePositionCommand>
{
    private readonly IPositionRepository _repository;

    public DeletePositionHandler(
        IPositionRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        DeletePositionCommand request,
        CancellationToken cancellationToken)
    {
        var position =
            await _repository.GetByIdAsync(request.Id);

        if (position == null)
            throw new DomainException("Position not found");

        await _repository.DeleteAsync(position);
    }
}
