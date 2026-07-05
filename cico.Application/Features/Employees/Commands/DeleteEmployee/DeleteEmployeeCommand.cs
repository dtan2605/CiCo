using MediatR;

namespace cico.Application.Features.Employees.Commands.DeleteEmployee;

public record DeleteEmployeeCommand(
    Guid Id
) : IRequest;