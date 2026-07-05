using MediatR;
using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Exceptions;

namespace cico.Application.Features.Notifications.Commands.MarkAsRead;

public class MarkAsReadHandler
    : IRequestHandler<MarkAsReadCommand>
{
    private readonly INotificationRepository _repository;

    public MarkAsReadHandler(
        INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        MarkAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var notification =
            await _repository.GetByIdAsync(
                request.Id);

        if (notification == null)
            throw new DomainException(
                "Notification not found");

        notification.IsRead = true;

        _repository.Update(notification);

        await _repository.SaveChangesAsync(
            cancellationToken);
    }
}
