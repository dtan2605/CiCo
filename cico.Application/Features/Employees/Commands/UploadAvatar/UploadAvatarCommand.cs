using MediatR;

namespace cico.Application.Features.Employees.Commands.UploadAvatar;

public record UploadAvatarCommand(
    Guid UserId,
    string ImageBase64
) : IRequest<string>;
