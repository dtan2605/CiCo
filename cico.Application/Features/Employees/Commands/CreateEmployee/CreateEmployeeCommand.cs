using MediatR;
using cico.Application.DTOs.Employees;

namespace cico.Application.Features.Employees.Commands.CreateEmployee;

public record CreateEmployeeCommand(
    string EmployeeCode,
    string FullName,
    DateTime DateOfBirth,
    string Email,
    string PhoneNumber,
    string Address,
    Guid DepartmentId,
    Guid PositionId,
    Guid UserId
) : IRequest<EmployeeDto>;