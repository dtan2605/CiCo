using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.EmployeeSchedules
    .Commands.UpdateEmployeeSchedule;

public class UpdateEmployeeScheduleHandler
    : IRequestHandler<UpdateEmployeeScheduleCommand>
{
    private readonly IEmployeeScheduleRepository _repository;

    public UpdateEmployeeScheduleHandler(
        IEmployeeScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        UpdateEmployeeScheduleCommand request,
        CancellationToken cancellationToken)
    {
        var entity =
            await _repository.GetByIdAsync(request.Id);

        if (entity == null)
            throw new DomainException(
                "EmployeeSchedule not found");

        entity.EmployeeId = request.EmployeeId;
        entity.ScheduleId = request.ScheduleId;
        entity.WorkDate = request.WorkDate;
        entity.IsOvertime = request.IsOvertime;
        entity.Note = request.Note ?? string.Empty;

        await _repository.UpdateAsync(entity);
    }
}
