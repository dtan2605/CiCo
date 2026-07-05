using AutoMapper;
using cico.Application.DTOs.Departments;
using cico.Domain.Entities;

namespace cico.Application.Mappings;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        CreateMap<Department, DepartmentDto>();
    }
}
