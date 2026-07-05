using MediatR;
using cico.Domain.Entities;
using cico.Domain.Enums;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Attendance.Commands.CheckOut;

public class CheckOutHandler
    : IRequestHandler<CheckOutCommand>
{
    private readonly IAttendanceRepository _attendanceRepo;
    private readonly IEmployeeScheduleRepository _scheduleRepo;
    private readonly IAttendanceLogRepository _logRepo;

    public CheckOutHandler(
        IAttendanceRepository attendanceRepo,
        IEmployeeScheduleRepository scheduleRepo,
        IAttendanceLogRepository logRepo)
    {
        _attendanceRepo = attendanceRepo;
        _scheduleRepo = scheduleRepo;
        _logRepo = logRepo;
    }

    public async Task Handle(
        CheckOutCommand request,
        CancellationToken cancellationToken)
    {
        var date = DateOnly.FromDateTime(
            request.CheckOutTime);

        var attendance =
            await _attendanceRepo.GetAttendanceAsync(
                request.EmployeeId,
                date);

        if (attendance == null)
            throw new DomainException(
                "No check-in record found for today");

        attendance.CheckOutTime =
            request.CheckOutTime;

        if (request.EmployeeScheduleId.HasValue
            || attendance.EmployeeScheduleId.HasValue)
        {
            var scheduleId = request.EmployeeScheduleId
                ?? attendance.EmployeeScheduleId!.Value;

            var empSchedule =
                await _scheduleRepo.GetByIdAsync(scheduleId);

            if (empSchedule?.Schedule != null)
            {
                var checkoutTimeOnly =
                    TimeOnly.FromDateTime(request.CheckOutTime);
                var endTime = empSchedule.Schedule.EndTime;

                if (checkoutTimeOnly < endTime)
                {
                    var early = (int)(endTime - checkoutTimeOnly).TotalMinutes;
                    if (early > 0)
                    {
                        await _logRepo.AddAsync(new AttendanceLog
                        {
                            AttendanceId = attendance.Id,
                            ScanTime = request.CheckOutTime,
                            Method = AttendanceMethod.Manual,
                            IsSuccess = true,
                            Message = $"Early departure: {early} min"
                        });
                    }
                }
                else if (checkoutTimeOnly > endTime)
                {
                    var overtime = (int)(checkoutTimeOnly - endTime).TotalMinutes;
                    if (overtime > 0)
                    {
                        await _logRepo.AddAsync(new AttendanceLog
                        {
                            AttendanceId = attendance.Id,
                            ScanTime = request.CheckOutTime,
                            Method = AttendanceMethod.Manual,
                            IsSuccess = true,
                            Message = $"Overtime: {overtime} min"
                        });
                    }
                }
            }
        }

        await _attendanceRepo.UpdateAsync(attendance);
    }
}
