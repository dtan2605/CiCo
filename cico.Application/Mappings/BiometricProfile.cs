using AutoMapper;
using cico.Application.DTOs.BiometricProfiles;
using cico.Domain.Entities;

namespace cico.Application.Mappings;

public class BiometricProfileMapping : Profile
{
    public BiometricProfileMapping()
    {
        CreateMap<BiometricProfile, BiometricProfileDto>()
            .ForMember(d => d.EmployeeName,
                o => o.MapFrom(
                    s => s.Employee.FullName))
            .ForMember(d => d.EmployeeCode,
                o => o.MapFrom(
                    s => s.Employee.EmployeeCode));
    }
}
