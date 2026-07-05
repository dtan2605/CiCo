using MediatR;
using cico.Application.DTOs.Positions;

namespace cico.Application.Features.Positions.Commands.CreatePosition;

public record CreatePositionCommand(
    string Name,
    string Description
) : IRequest<PositionDto>;
