using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Notifications
    .Commands.DeleteNotification;

public class DeleteNotificationHandler
    : IRequestHandler<DeleteNotificationCommand>
{
    private readonly INotificationRepository _repository;

    public DeleteNotificationHandler(
        INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        DeleteNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var notification =
            await _repository.GetByIdAsync(
                request.Id);

        if (notification == null)
            throw new DomainException(
                "Notification not found");

        await _repository.DeleteAsync(notification);
    }
}
