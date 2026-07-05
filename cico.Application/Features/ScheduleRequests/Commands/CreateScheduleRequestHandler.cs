using MediatR;
using cico.Domain.Entities;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.ScheduleRequests.Commands;

public class CreateScheduleRequestHandler : IRequestHandler<CreateScheduleRequestCommand, Guid>
{
    private readonly IScheduleRequestRepository _repo;

    public CreateScheduleRequestHandler(IScheduleRequestRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> Handle(CreateScheduleRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = new ScheduleRequest
        {
            EmployeeId = request.EmployeeId,
            RequestDate = request.RequestDate,
            CurrentScheduleId = request.CurrentScheduleId,
            RequestedScheduleId = request.RequestedScheduleId,
            Reason = request.Reason,
            Status = Domain.Enums.ScheduleRequestStatus.Pending
        };

        await _repo.AddAsync(entity);
        return entity.Id;
    }
}
