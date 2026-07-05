using MediatR;
using cico.Domain.Entities;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Notifications.Commands.CreateBroadcastNotification;

public class CreateBroadcastNotificationHandler : IRequestHandler<CreateBroadcastNotificationCommand>
{
    private readonly INotificationRepository _notificationRepo;
    private readonly IEmployeeRepository _employeeRepo;

    public CreateBroadcastNotificationHandler(
        INotificationRepository notificationRepo,
        IEmployeeRepository employeeRepo)
    {
        _notificationRepo = notificationRepo;
        _employeeRepo = employeeRepo;
    }

    public async Task Handle(CreateBroadcastNotificationCommand request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepo.GetActiveEmployeesAsync();

        foreach (var emp in employees)
        {
            var notification = new Notification
            {
                EmployeeId = emp.Id,
                Title = request.Title,
                Content = request.Content,
                Type = request.Type,
                IsRead = false
            };
            await _notificationRepo.AddAsync(notification);
        }
    }
}
