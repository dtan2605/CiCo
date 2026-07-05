using AutoMapper;
using MediatR;
using cico.Domain.Entities;
using cico.Application.DTOs.Positions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Positions.Commands.CreatePosition;

public class CreatePositionHandler
    : IRequestHandler<CreatePositionCommand, PositionDto>
{
    private readonly IPositionRepository _repository;
    private readonly IMapper _mapper;

    public CreatePositionHandler(
        IPositionRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PositionDto> Handle(
        CreatePositionCommand request,
        CancellationToken cancellationToken)
    {
        var position = new Position
        {
            Name = request.Name,
            Description = request.Description
        };

        await _repository.AddAsync(position);

        return _mapper.Map<PositionDto>(position);
    }
}
