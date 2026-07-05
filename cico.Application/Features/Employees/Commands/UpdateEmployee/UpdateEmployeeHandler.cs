using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeHandler
    : IRequestHandler<UpdateEmployeeCommand>
{
    private readonly IEmployeeRepository _repository;

    public UpdateEmployeeHandler(
        IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        UpdateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var employee =
            await _repository.GetByIdAsync(request.Id);

        if (employee == null)
            throw new DomainException("Employee not found");

        employee.FullName = request.FullName;
        employee.Email = request.Email;
        employee.PhoneNumber = request.PhoneNumber;
        employee.Address = request.Address;
        employee.DepartmentId = request.DepartmentId;
        employee.PositionId = request.PositionId;
        employee.IsActive = request.IsActive;

        await _repository.UpdateAsync(employee);
    }
}