using AutoMapper;
using cico.Application.DTOs.Reports;
using cico.Domain.Entities;
using cico.Domain.Enums;

namespace cico.Application.Mappings;

public class ReportProfile : Profile
{
    public ReportProfile()
    {
        CreateMap<Attendance, AttendanceBriefDto>()
            .ForMember(d => d.EmployeeName,
                o => o.MapFrom(
                    s => s.Employee.FullName))
            .ForMember(d => d.EmployeeCode,
                o => o.MapFrom(
                    s => s.Employee.EmployeeCode))
            .ForMember(d => d.Status,
                o => o.MapFrom(
                    s => s.Status.ToString()));
    }
}
