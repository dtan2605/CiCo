using MediatR;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Notifications
    .Commands.MarkAllAsRead;

public class MarkAllAsReadHandler
    : IRequestHandler<MarkAllAsReadCommand>
{
    private readonly INotificationRepository _repository;

    public MarkAllAsReadHandler(
        INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        MarkAllAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var unread =
            await _repository.GetUnreadAsync(
                request.UserId);

        foreach (var notification in unread)
            notification.IsRead = true;

        foreach (var notification in unread)
            _repository.Update(notification);

        if (unread.Count > 0)
            await _repository.SaveChangesAsync(
                cancellationToken);
    }
}
