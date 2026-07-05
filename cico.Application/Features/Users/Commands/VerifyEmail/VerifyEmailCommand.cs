using MediatR;

namespace cico.Application.Features.Users.Commands.VerifyEmail{
    public record VerifyEmailCommand(
    string Token
) : IRequest;
}