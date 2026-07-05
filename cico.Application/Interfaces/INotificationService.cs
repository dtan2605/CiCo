namespace cico.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(
        Guid userId,
        string title,
        string content,
        string type);
}
