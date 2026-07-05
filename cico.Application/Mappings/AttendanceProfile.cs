using AutoMapper;
using cico.Application.DTOs.Attendances;
using cico.Domain.Entities;

namespace cico.Application.Mappings;

public class AttendanceProfile : Profile
{
    public AttendanceProfile()
    {
        CreateMap<Attendance, AttendanceDto>()
            .ForMember(d => d.EmployeeCode,
                o => o.MapFrom(s => s.Employee != null
                    ? s.Employee.EmployeeCode : null))
            .ForMember(d => d.EmployeeName,
                o => o.MapFrom(s => s.Employee != null
                    ? s.Employee.FullName : null));
    }
}
