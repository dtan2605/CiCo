using MediatR;
using cico.Application.DTOs.AttendanceLogs;

namespace cico.Application.Features.AttendanceLogs
    .Queries.GetAttendanceLogById;

public record GetAttendanceLogByIdQuery(
    Guid Id
) : IRequest<AttendanceLogDto?>;
