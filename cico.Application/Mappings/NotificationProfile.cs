using AutoMapper;
using cico.Application.DTOs.Notifications;
using cico.Domain.Entities;

namespace cico.Application.Mappings;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationDto>();
    }
}
