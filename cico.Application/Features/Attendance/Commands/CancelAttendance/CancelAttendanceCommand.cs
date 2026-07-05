using MediatR;

namespace cico.Application.Features.Attendance.Commands.CancelAttendance;

public record CancelAttendanceCommand(
    Guid AttendanceId,
    string SecurityCode
) : IRequest;
