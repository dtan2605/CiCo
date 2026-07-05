using AutoMapper;
using cico.Domain.Entities;
using cico.Application.Features.ScheduleRequests.Queries;

namespace cico.Application.Mappings;

public class ScheduleRequestProfile : Profile
{
    public ScheduleRequestProfile()
    {
        CreateMap<ScheduleRequest, ScheduleRequestDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee.FullName))
            .ForMember(d => d.RequestDate, opt => opt.MapFrom(s => s.RequestDate.ToString("yyyy-MM-dd")))
            .ForMember(d => d.CurrentScheduleName, opt => opt.MapFrom(s => s.CurrentSchedule != null ? s.CurrentSchedule.Name : null))
            .ForMember(d => d.RequestedScheduleName, opt => opt.MapFrom(s => s.RequestedSchedule != null ? s.RequestedSchedule.Name : null))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => (int)s.Status))
            .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => s.CreatedAt.ToString("yyyy-MM-dd HH:mm")))
            .ForMember(d => d.ResolvedAt, opt => opt.MapFrom(s => s.ResolvedAt.HasValue ? s.ResolvedAt.Value.ToString("yyyy-MM-dd HH:mm") : null));
    }
}
