using MediatR;

namespace cico.Application.Features.Notifications
    .Commands.DeleteNotification;

public record DeleteNotificationCommand(
    Guid Id
) : IRequest;
