using AutoMapper;
using cico.Application.DTOs.Positions;
using cico.Domain.Entities;

namespace cico.Application.Mappings;

public class PositionProfile : Profile
{
    public PositionProfile()
    {
        CreateMap<Position, PositionDto>();
    }
}
