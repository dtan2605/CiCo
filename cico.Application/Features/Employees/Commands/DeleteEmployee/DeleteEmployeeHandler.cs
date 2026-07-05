using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeHandler
    : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly IEmployeeRepository _repository;

    public DeleteEmployeeHandler(
        IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        DeleteEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var employee =
            await _repository.GetByIdAsync(request.Id);

        if (employee == null)
            throw new DomainException("Employee not found");

        employee.IsDeleted = true;
        employee.IsActive = false;

        await _repository.UpdateAsync(employee);
    }
}