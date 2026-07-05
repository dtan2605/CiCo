using MediatR;

namespace cico.Application.Features.Attendance.Commands.CheckOut;

public record CheckOutCommand(
    Guid EmployeeId,
    DateTime CheckOutTime,
    Guid? EmployeeScheduleId
) : IRequest;
