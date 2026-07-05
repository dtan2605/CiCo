using MediatR;

namespace cico.Application.Features.Notifications.Commands.MarkAsRead;

public record MarkAsReadCommand(
    Guid Id
) : IRequest;
