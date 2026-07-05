using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Schedules.Commands.UpdateSchedule;

public class UpdateScheduleHandler
    : IRequestHandler<UpdateScheduleCommand>
{
    private readonly IScheduleRepository _repository;

    public UpdateScheduleHandler(
        IScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        UpdateScheduleCommand request,
        CancellationToken cancellationToken)
    {
        var schedule =
            await _repository.GetByIdAsync(request.Id);

        if (schedule == null)
            throw new DomainException("Schedule not found");

        schedule.Name = request.Name;
        schedule.StartTime = request.StartTime;
        schedule.EndTime = request.EndTime;
        schedule.LateThresholdMinutes = request.LateThresholdMinutes;
        schedule.IsActive = request.IsActive;

        await _repository.UpdateAsync(schedule);
    }
}
