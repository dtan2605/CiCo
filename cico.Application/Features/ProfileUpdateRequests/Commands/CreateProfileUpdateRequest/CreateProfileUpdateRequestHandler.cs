using MediatR;
using cico.Domain.Entities;
using cico.Domain.Enums;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.ProfileUpdateRequests.Commands.CreateProfileUpdateRequest;

public class CreateProfileUpdateRequestHandler
    : IRequestHandler<CreateProfileUpdateRequestCommand, Guid>
{
    private readonly IProfileUpdateRequestRepository _repo;

    public CreateProfileUpdateRequestHandler(
        IProfileUpdateRequestRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> Handle(
        CreateProfileUpdateRequestCommand request,
        CancellationToken cancellationToken)
    {
        var entity = new ProfileUpdateRequest
        {
            EmployeeId = request.EmployeeId,
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            Status = ProfileUpdateStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(entity);
        return entity.Id;
    }
}
