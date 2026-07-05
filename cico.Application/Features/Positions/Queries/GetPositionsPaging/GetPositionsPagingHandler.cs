using AutoMapper;
using MediatR;
using cico.Application.DTOs.Positions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Positions.Queries.GetPositionsPaging;

public class GetPositionsPagingHandler
    : IRequestHandler<
        GetPositionsPagingQuery,
        List<PositionDto>>
{
    private readonly IPositionRepository _repository;
    private readonly IMapper _mapper;

    public GetPositionsPagingHandler(
        IPositionRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<PositionDto>>
        Handle(
            GetPositionsPagingQuery request,
            CancellationToken cancellationToken)
    {
        var positions =
            await _repository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            positions = positions
                .Where(p =>
                    p.Name.Contains(
                        request.Keyword,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var result = positions
            .Skip(
                (request.PageNumber - 1)
                * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return _mapper.Map<List<PositionDto>>(
            result);
    }
}
