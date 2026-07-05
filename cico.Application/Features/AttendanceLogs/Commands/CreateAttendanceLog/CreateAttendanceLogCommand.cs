using MediatR;
using cico.Application.DTOs.AttendanceLogs;
using cico.Domain.Enums;

namespace cico.Application.Features.AttendanceLogs
    .Commands.CreateAttendanceLog;

public record CreateAttendanceLogCommand(
    DateTime ScanTime,
    bool IsSuccess,
    string? Message,
    Guid AttendanceId,
    Guid DeviceId,
    AttendanceMethod Method
) : IRequest<AttendanceLogDto>;
