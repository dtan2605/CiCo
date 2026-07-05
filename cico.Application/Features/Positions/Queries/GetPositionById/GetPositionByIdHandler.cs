using AutoMapper;
using MediatR;
using cico.Application.DTOs.Positions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Positions.Queries.GetPositionById;

public class GetPositionByIdHandler
    : IRequestHandler<GetPositionByIdQuery, PositionDto?>
{
    private readonly IPositionRepository _repository;
    private readonly IMapper _mapper;

    public GetPositionByIdHandler(
        IPositionRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PositionDto?> Handle(
        GetPositionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var position =
            await _repository.GetByIdAsync(
                request.Id);

        if (position == null)
            return null;

        return _mapper.Map<PositionDto>(
            position);
    }
}
