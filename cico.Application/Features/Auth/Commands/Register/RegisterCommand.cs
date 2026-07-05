using MediatR;

namespace cico.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FullName
) : IRequest<Guid>;