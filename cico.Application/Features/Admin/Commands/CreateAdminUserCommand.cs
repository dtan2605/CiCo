using MediatR;

namespace cico.Application.Features.Admin.Commands;

public record CreateAdminUserCommand(
    string Username,
    string Email,
    string Password,
    Guid RoleId,
    string FullName,
    string? EmployeeCode,
    string? PhoneNumber,
    Guid? DepartmentId,
    Guid? PositionId
) : IRequest<Guid>;
