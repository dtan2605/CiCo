using MediatR;
using cico.Domain.Exceptions;
using cico.Domain.Enums;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.LeaveRequests.Commands.ResolveLeaveRequest;

public class ResolveLeaveRequestHandler
    : IRequestHandler<ResolveLeaveRequestCommand>
{
    private readonly ILeaveRequestRepository _repo;

    public ResolveLeaveRequestHandler(
        ILeaveRequestRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(
        ResolveLeaveRequestCommand request,
        CancellationToken cancellationToken)
    {
        var leaveRequest =
            await _repo.GetByIdAsync(request.RequestId);

        if (leaveRequest == null)
            throw new DomainException("Leave request not found");

        if (leaveRequest.Status != LeaveStatus.Pending)
            throw new DomainException("Request has already been resolved");

        leaveRequest.Status = request.Status;
        await _repo.UpdateAsync(leaveRequest);
    }
}
