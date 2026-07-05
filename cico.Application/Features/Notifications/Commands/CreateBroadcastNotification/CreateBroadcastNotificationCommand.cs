using MediatR;
using cico.Domain.Enums;

namespace cico.Application.Features.Notifications.Commands.CreateBroadcastNotification;

public record CreateBroadcastNotificationCommand(
    string Title,
    string Content,
    NotificationType Type
) : IRequest;
