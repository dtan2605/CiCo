using MediatR;
using cico.Application.DTOs.Employees;

namespace cico.Application.Features.Employees.Queries.GetEmployeeByUserId;

public record GetEmployeeByUserIdQuery(Guid UserId) : IRequest<EmployeeDto?>;
