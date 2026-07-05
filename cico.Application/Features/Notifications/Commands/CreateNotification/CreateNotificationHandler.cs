using AutoMapper;
using MediatR;
using cico.Domain.Entities;
using cico.Application.DTOs.Notifications;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Notifications
    .Commands.CreateNotification;

public class CreateNotificationHandler
    : IRequestHandler<
        CreateNotificationCommand,
        NotificationDto>
{
    private readonly INotificationRepository _repository;
    private readonly IMapper _mapper;

    public CreateNotificationHandler(
        INotificationRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<NotificationDto> Handle(
        CreateNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            EmployeeId = request.EmployeeId,
            Title = request.Title,
            Content = request.Content,
            Type = request.Type,
            IsRead = false
        };

        await _repository.AddAsync(notification);

        return _mapper.Map<NotificationDto>(
            notification);
    }
}
