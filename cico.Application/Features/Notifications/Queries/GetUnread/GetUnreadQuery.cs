using MediatR;
using cico.Application.DTOs.Notifications;

namespace cico.Application.Features.Notifications.Queries.GetUnread;

public record GetUnreadQuery(
    Guid UserId
) : IRequest<IReadOnlyList<NotificationDto>>;
