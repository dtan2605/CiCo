using MediatR;
using cico.Application.DTOs.Employees;

namespace cico.Application.Features.Employees.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery(
    Guid Id
) : IRequest<EmployeeDto?>;