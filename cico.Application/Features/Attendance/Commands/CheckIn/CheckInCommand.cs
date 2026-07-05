using MediatR;
using cico.Domain.Enums;
using cico.Application.DTOs.Attendances;

namespace cico.Application.Features.Attendance.Commands.CheckIn;

public record CheckInCommand(
    Guid EmployeeId,
    DateTime CheckInTime,
    AttendanceMethod Method,
    Guid? EmployeeScheduleId
) : IRequest<AttendanceDto>;
