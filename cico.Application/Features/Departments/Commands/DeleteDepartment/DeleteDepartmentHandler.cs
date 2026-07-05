using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Departments.Commands.DeleteDepartment;

public class DeleteDepartmentHandler
    : IRequestHandler<DeleteDepartmentCommand>
{
    private readonly IDepartmentRepository _repository;

    public DeleteDepartmentHandler(
        IDepartmentRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        DeleteDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        var department =
            await _repository.GetByIdAsync(request.Id);

        if (department == null)
            throw new DomainException("Department not found");

        await _repository.DeleteAsync(department);
    }
}
