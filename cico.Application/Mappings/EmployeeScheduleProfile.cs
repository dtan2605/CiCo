using AutoMapper;
using cico.Application.DTOs.EmployeeSchedules;
using cico.Domain.Entities;

namespace cico.Application.Mappings;

public class EmployeeScheduleProfile : Profile
{
    public EmployeeScheduleProfile()
    {
        CreateMap<EmployeeSchedule, EmployeeScheduleDto>()
            .ForMember(d => d.EmployeeName,
                o => o.MapFrom(
                    s => s.Employee.FullName))
            .ForMember(d => d.ScheduleName,
                o => o.MapFrom(
                    s => s.Schedule.Name))
            .ForMember(d => d.StartTime,
                o => o.MapFrom(
                    s => s.Schedule.StartTime))
            .ForMember(d => d.EndTime,
                o => o.MapFrom(
                    s => s.Schedule.EndTime));
    }
}
