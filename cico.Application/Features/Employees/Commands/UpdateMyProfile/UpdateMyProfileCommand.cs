using MediatR;

namespace cico.Application.Features.Employees.Commands.UpdateMyProfile;

public record UpdateMyProfileCommand(
    Guid UserId,
    string FullName,
    string Email,
    string PhoneNumber,
    string Address
) : IRequest;
