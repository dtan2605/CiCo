using MediatR;
using cico.Application.DTOs.Notifications;
using cico.Domain.Enums;

namespace cico.Application.Features.Notifications
    .Commands.CreateNotification;

public record CreateNotificationCommand(
    Guid EmployeeId,
    string Title,
    string Content,
    NotificationType Type
) : IRequest<NotificationDto>;
