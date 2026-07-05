using MediatR;
using cico.Application.DTOs.Departments;

namespace cico.Application.Features.Departments.Queries.GetDepartmentById;

public record GetDepartmentByIdQuery(
    Guid Id
) : IRequest<DepartmentDto?>;
