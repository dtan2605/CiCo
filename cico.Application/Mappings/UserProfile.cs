using AutoMapper;
using cico.Application.DTOs.Users;
using cico.Application.DTOs.Admin;
using cico.Domain.Entities;

namespace cico.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(
                d => d.RoleName,
                opt => opt.MapFrom(
                    s => s.Role.Name));

        CreateMap<User, AdminUserDto>()
            .ForMember(d => d.RoleName, opt => opt.MapFrom(s => s.Role.Name))
            .ForMember(d => d.EmployeeId, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.Id : (Guid?)null))
            .ForMember(d => d.EmployeeCode, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.EmployeeCode : null))
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null))
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.PhoneNumber : null))
            .ForMember(d => d.DepartmentId, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.DepartmentId : (Guid?)null))
            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.Department.Name : null))
            .ForMember(d => d.PositionId, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.PositionId : (Guid?)null))
            .ForMember(d => d.PositionName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.Position.Name : null));

        CreateMap<Role, RoleDto>();

        CreateMap<CreateUserDto, User>();

        CreateMap<UpdateUserDto, User>();
    }
}