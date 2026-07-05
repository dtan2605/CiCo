using MediatR;

namespace cico.Application.Features.Admin.Commands;

public record UpdateAdminUserCommand(
    Guid Id,
    string? Username,
    string? Email,
    Guid? RoleId,
    bool? IsActive,
    string? FullName,
    string? PhoneNumber,
    Guid? DepartmentId,
    Guid? PositionId
) : IRequest;
