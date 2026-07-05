using MediatR;

namespace cico.Application.Features.Positions.Commands.DeletePosition;

public record DeletePositionCommand(
    Guid Id
) : IRequest;
