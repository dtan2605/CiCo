using MediatR;

namespace cico.Application.Features.Positions.Commands.UpdatePosition;

public record UpdatePositionCommand(
    Guid Id,
    string Name,
    string Description
) : IRequest;
