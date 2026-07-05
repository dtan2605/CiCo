using AutoMapper;
using MediatR;
using cico.Application.DTOs.Notifications;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Notifications
    .Queries.GetNotificationsPaging;

public class GetNotificationsPagingHandler
    : IRequestHandler<
        GetNotificationsPagingQuery,
        List<NotificationDto>>
{
    private readonly INotificationRepository _repository;
    private readonly IMapper _mapper;

    public GetNotificationsPagingHandler(
        INotificationRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<NotificationDto>> Handle(
        GetNotificationsPagingQuery request,
        CancellationToken cancellationToken)
    {
        var all =
            await _repository.GetAllAsync();

        var filtered = all
            .Where(x =>
                x.EmployeeId == request.EmployeeId &&
                (!request.IsRead.HasValue ||
                    x.IsRead == request.IsRead.Value) &&
                (!request.Type.HasValue ||
                    x.Type == request.Type.Value))
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.PageNumber - 1)
                * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return _mapper.Map<List<NotificationDto>>(
            filtered);
    }
}
