using MediatR;
using cico.Application.DTOs.Notifications;
using cico.Domain.Enums;

namespace cico.Application.Features.Notifications
    .Queries.GetNotificationsPaging;

public record GetNotificationsPagingQuery(
    Guid EmployeeId,
    int PageNumber = 1,
    int PageSize = 20,
    bool? IsRead = null,
    NotificationType? Type = null
) : IRequest<List<NotificationDto>>;
