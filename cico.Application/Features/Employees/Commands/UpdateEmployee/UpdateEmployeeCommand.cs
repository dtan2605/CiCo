using MediatR;

namespace cico.Application.Features.Employees.Commands.UpdateEmployee;

public record UpdateEmployeeCommand(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string Address,
    Guid DepartmentId,
    Guid PositionId,
    bool IsActive
) : IRequest;