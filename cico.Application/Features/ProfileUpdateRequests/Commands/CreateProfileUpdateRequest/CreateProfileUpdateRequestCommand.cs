using MediatR;

namespace cico.Application.Features.ProfileUpdateRequests.Commands.CreateProfileUpdateRequest;

public record CreateProfileUpdateRequestCommand(
    Guid EmployeeId,
    string FullName,
    string Email,
    string PhoneNumber,
    string Address
) : IRequest<Guid>;
