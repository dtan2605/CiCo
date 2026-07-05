using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Schedules.Commands.DeleteSchedule;

public class DeleteScheduleHandler
    : IRequestHandler<DeleteScheduleCommand>
{
    private readonly IScheduleRepository _repository;

    public DeleteScheduleHandler(
        IScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        DeleteScheduleCommand request,
        CancellationToken cancellationToken)
    {
        var schedule =
            await _repository.GetByIdAsync(request.Id);

        if (schedule == null)
            throw new DomainException("Schedule not found");

        await _repository.DeleteAsync(schedule);
    }
}
