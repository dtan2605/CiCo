using MediatR;
using cico.Application.DTOs.AttendanceLogs;

namespace cico.Application.Features.AttendanceLogs
    .Queries.GetAttendanceLogsByDevice;

public record GetAttendanceLogsByDeviceQuery(
    Guid DeviceId
) : IRequest<List<AttendanceLogDto>>;
