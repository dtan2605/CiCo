using AutoMapper;
using cico.Application.DTOs.Schedules;
using cico.Domain.Entities;

namespace cico.Application.Mappings;

public class ScheduleProfile : Profile
{
    public ScheduleProfile()
    {
        CreateMap<Schedule, ScheduleDto>();
    }
}
