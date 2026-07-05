using MediatR;
using cico.Application.Features.Auth.Commands.Login;
namespace cico.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<LoginResponse>
{
    public string RefreshToken { get; set; } = string.Empty;
}