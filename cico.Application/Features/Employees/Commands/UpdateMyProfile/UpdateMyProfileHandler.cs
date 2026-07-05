using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Employees.Commands.UpdateMyProfile;

public class UpdateMyProfileHandler
    : IRequestHandler<UpdateMyProfileCommand>
{
    private readonly IEmployeeRepository _repository;

    public UpdateMyProfileHandler(
        IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        UpdateMyProfileCommand request,
        CancellationToken cancellationToken)
    {
        var employee =
            await _repository.GetByUserIdAsync(request.UserId);

        if (employee == null)
            throw new DomainException("Employee not found");

        employee.FullName = request.FullName;
        employee.Email = request.Email;
        employee.PhoneNumber = request.PhoneNumber;
        employee.Address = request.Address;

        await _repository.UpdateAsync(employee);
    }
}
