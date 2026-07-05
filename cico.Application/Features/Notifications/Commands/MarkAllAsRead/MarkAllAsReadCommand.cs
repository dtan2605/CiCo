using MediatR;

namespace cico.Application.Features.Notifications
    .Commands.MarkAllAsRead;

public record MarkAllAsReadCommand(
    Guid UserId
) : IRequest;
