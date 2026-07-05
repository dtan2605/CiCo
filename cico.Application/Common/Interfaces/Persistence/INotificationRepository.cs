using cico.Domain.Entities;

namespace cico.Application.Common.Interfaces.Persistence;

public interface INotificationRepository
    : IBaseRepository<Notification>
{
    Task<IReadOnlyList<Notification>>
        GetUnreadAsync(Guid userId);
}