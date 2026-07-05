using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Positions.Commands.UpdatePosition;

public class UpdatePositionHandler
    : IRequestHandler<UpdatePositionCommand>
{
    private readonly IPositionRepository _repository;

    public UpdatePositionHandler(
        IPositionRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        UpdatePositionCommand request,
        CancellationToken cancellationToken)
    {
        var position =
            await _repository.GetByIdAsync(request.Id);

        if (position == null)
            throw new DomainException("Position not found");

        position.Name = request.Name;
        position.Description = request.Description;

        await _repository.UpdateAsync(position);
    }
}
