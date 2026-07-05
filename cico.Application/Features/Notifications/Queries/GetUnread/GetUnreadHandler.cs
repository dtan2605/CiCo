using AutoMapper;
using MediatR;
using cico.Application.DTOs.Notifications;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Notifications.Queries.GetUnread;

public class GetUnreadHandler
    : IRequestHandler<GetUnreadQuery, IReadOnlyList<NotificationDto>>
{
    private readonly INotificationRepository _repository;
    private readonly IMapper _mapper;

    public GetUnreadHandler(
        INotificationRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<NotificationDto>> Handle(
        GetUnreadQuery request,
        CancellationToken cancellationToken)
    {
        var notifications =
            await _repository.GetUnreadAsync(
                request.UserId);

        return _mapper.Map<IReadOnlyList<NotificationDto>>(
            notifications);
    }
}
