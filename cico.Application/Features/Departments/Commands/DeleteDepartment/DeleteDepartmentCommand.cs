using MediatR;

namespace cico.Application.Features.Departments.Commands.DeleteDepartment;

public record DeleteDepartmentCommand(
    Guid Id
) : IRequest;
