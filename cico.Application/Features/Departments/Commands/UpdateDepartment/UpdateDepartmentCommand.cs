using MediatR;

namespace cico.Application.Features.Departments.Commands.UpdateDepartment;

public record UpdateDepartmentCommand(
    Guid Id,
    string Name,
    string Description,
    bool IsActive
) : IRequest;
