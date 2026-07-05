using AutoMapper;
using cico.Application.DTOs.AttendanceLogs;
using cico.Domain.Entities;

namespace cico.Application.Mappings;

public class AttendanceLogProfile : Profile
{
    public AttendanceLogProfile()
    {
        CreateMap<AttendanceLog, AttendanceLogDto>()
            .ForMember(d => d.AttendanceDate,
                o => o.MapFrom(
                    s => s.Attendance.AttendanceDate
                        .ToString("yyyy-MM-dd")))
            .ForMember(d => d.EmployeeName,
                o => o.MapFrom(
                    s => s.Attendance.Employee.FullName))
            .ForMember(d => d.DeviceName,
                o => o.MapFrom(
                    s => s.Device.Name))
            .ForMember(d => d.DeviceCode,
                o => o.MapFrom(
                    s => s.Device.DeviceCode));
    }
}
