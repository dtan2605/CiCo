using MediatR;
using cico.Domain.Entities;
using cico.Domain.Enums;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.LeaveRequests.Commands.CreateLeaveRequest;

public class CreateLeaveRequestHandler
    : IRequestHandler<CreateLeaveRequestCommand, Guid>
{
    private readonly ILeaveRequestRepository _repo;

    public CreateLeaveRequestHandler(
        ILeaveRequestRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> Handle(
        CreateLeaveRequestCommand request,
        CancellationToken cancellationToken)
    {
        var entity = new LeaveRequest
        {
            EmployeeId = request.EmployeeId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            Reason = request.Reason,
            Status = LeaveStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(entity);
        return entity.Id;
    }
}
