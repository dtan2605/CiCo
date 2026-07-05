using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentHandler
    : IRequestHandler<UpdateDepartmentCommand>
{
    private readonly IDepartmentRepository _repository;

    public UpdateDepartmentHandler(
        IDepartmentRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        UpdateDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        var department =
            await _repository.GetByIdAsync(request.Id);

        if (department == null)
            throw new DomainException("Department not found");

        department.Name = request.Name;
        department.Description = request.Description;

        await _repository.UpdateAsync(department);
    }
}
