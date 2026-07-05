using MediatR;
using cico.Application.DTOs.Departments;

namespace cico.Application.Features.Departments.Commands.CreateDepartment;

public record CreateDepartmentCommand(
    string Name,
    string Description
) : IRequest<DepartmentDto>;
