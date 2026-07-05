using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.ScheduleRequests.Commands;

public class ResolveScheduleRequestHandler : IRequestHandler<ResolveScheduleRequestCommand>
{
    private readonly IScheduleRequestRepository _repo;

    public ResolveScheduleRequestHandler(IScheduleRequestRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(ResolveScheduleRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repo.GetByIdAsync(request.Id);
        if (entity == null)
            throw new DomainException("Schedule request not found");

        entity.Status = request.Status;
        entity.AdminNote = request.AdminNote;
        entity.ResolvedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(entity);
    }
}
