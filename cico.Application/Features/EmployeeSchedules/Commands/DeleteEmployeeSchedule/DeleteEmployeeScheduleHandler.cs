using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.EmployeeSchedules
    .Commands.DeleteEmployeeSchedule;

public class DeleteEmployeeScheduleHandler
    : IRequestHandler<DeleteEmployeeScheduleCommand>
{
    private readonly IEmployeeScheduleRepository _repository;

    public DeleteEmployeeScheduleHandler(
        IEmployeeScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        DeleteEmployeeScheduleCommand request,
        CancellationToken cancellationToken)
    {
        var entity =
            await _repository.GetByIdAsync(request.Id);

        if (entity == null)
            throw new DomainException(
                "EmployeeSchedule not found");

        await _repository.DeleteAsync(entity);
    }
}
