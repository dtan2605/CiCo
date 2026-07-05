using AutoMapper;
using cico.Application.DTOs.Employees;
using cico.Domain.Entities;

namespace cico.Application.Mappings;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<Employee, EmployeeDto>()
            .ForMember(
                dest => dest.DepartmentName,
                opt => opt.MapFrom(src => src.Department.Name))

            .ForMember(
                dest => dest.PositionName,
                opt => opt.MapFrom(src => src.Position.Name));
    }
}