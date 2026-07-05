using MediatR;
using cico.Application.DTOs.Positions;

namespace cico.Application.Features.Positions.Queries.GetPositionsPaging;

public record GetPositionsPagingQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Keyword = null
) : IRequest<List<PositionDto>>;
