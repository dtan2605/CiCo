using MediatR;
using cico.Application.DTOs.AttendanceLogs;
using cico.Domain.Enums;

namespace cico.Application.Features.AttendanceLogs
    .Queries.GetAttendanceLogsPaging;

public record GetAttendanceLogsPagingQuery(
    int PageNumber = 1,
    int PageSize = 20,
    Guid? AttendanceId = null,
    Guid? DeviceId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    bool? IsSuccess = null
) : IRequest<List<AttendanceLogDto>>;
