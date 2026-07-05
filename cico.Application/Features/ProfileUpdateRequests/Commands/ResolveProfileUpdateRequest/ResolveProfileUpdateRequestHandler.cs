using MediatR;
using cico.Domain.Exceptions;
using cico.Domain.Enums;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.ProfileUpdateRequests.Commands.ResolveProfileUpdateRequest;

public class ResolveProfileUpdateRequestHandler
    : IRequestHandler<ResolveProfileUpdateRequestCommand>
{
    private readonly IProfileUpdateRequestRepository _repo;
    private readonly IEmployeeRepository _employeeRepo;

    public ResolveProfileUpdateRequestHandler(
        IProfileUpdateRequestRepository repo,
        IEmployeeRepository employeeRepo)
    {
        _repo = repo;
        _employeeRepo = employeeRepo;
    }

    public async Task Handle(
        ResolveProfileUpdateRequestCommand request,
        CancellationToken cancellationToken)
    {
        var profileRequest =
            await _repo.GetByIdAsync(request.RequestId);

        if (profileRequest == null)
            throw new DomainException("Profile update request not found");

        if (profileRequest.Status != ProfileUpdateStatus.Pending)
            throw new DomainException("Request has already been resolved");

        profileRequest.Status = request.Status;
        profileRequest.ResolvedByUserId = request.ResolvedByUserId;
        profileRequest.ResolvedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(profileRequest);

        if (request.Status == ProfileUpdateStatus.Approved)
        {
            var employee =
                await _employeeRepo.GetByIdAsync(profileRequest.EmployeeId);

            if (employee != null)
            {
                employee.FullName = profileRequest.FullName;
                employee.Email = profileRequest.Email;
                employee.PhoneNumber = profileRequest.PhoneNumber;
                employee.Address = profileRequest.Address;

                await _employeeRepo.UpdateAsync(employee);
            }
        }
    }
}
