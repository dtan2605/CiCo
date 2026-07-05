using MediatR;
using cico.Application.DTOs.Positions;

namespace cico.Application.Features.Positions.Queries.GetPositionById;

public record GetPositionByIdQuery(
    Guid Id
) : IRequest<PositionDto?>;
