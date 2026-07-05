using MediatR;

namespace cico.Application.Features.Users.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(
    string Email
) : IRequest;
}