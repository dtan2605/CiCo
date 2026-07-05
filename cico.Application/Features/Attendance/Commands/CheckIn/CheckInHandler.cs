using AutoMapper;
using MediatR;
using cico.Domain.Entities;
using cico.Domain.Enums;
using cico.Application.DTOs.Attendances;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Attendance.Commands.CheckIn;

public class CheckInHandler
    : IRequestHandler<CheckInCommand, AttendanceDto>
{
    private readonly IAttendanceRepository _repository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IMapper _mapper;

    public CheckInHandler(
        IAttendanceRepository repository,
        IScheduleRepository scheduleRepository,
        IMapper mapper)
    {
        _repository = repository;
        _scheduleRepository = scheduleRepository;
        _mapper = mapper;
    }

    public async Task<AttendanceDto> Handle(
        CheckInCommand request,
        CancellationToken cancellationToken)
    {
        var date = DateOnly.FromDateTime(
            request.CheckInTime);

        var existing =
            await _repository.GetAttendanceAsync(
                request.EmployeeId,
                date);

        if (existing != null)
            return _mapper.Map<AttendanceDto>(
                existing);

        var lateMinutes = 0;
        var status = AttendanceStatus.Present;

        if (request.EmployeeScheduleId.HasValue)
        {
            var schedule =
                await _scheduleRepository
                    .GetByIdAsync(
                        request.EmployeeScheduleId.Value);

            if (schedule != null)
            {
                var checkInTimeOnly =
                    TimeOnly.FromDateTime(
                        request.CheckInTime);

                var diff = checkInTimeOnly -
                    schedule.StartTime;

                if (diff.TotalMinutes >
                    schedule.LateThresholdMinutes)
                {
                    lateMinutes = (int)diff.TotalMinutes;
                    status = AttendanceStatus.Late;
                }
            }
        }

        var attendance = new Domain.Entities.Attendance
        {
            AttendanceDate = date,
            CheckInTime = request.CheckInTime,
            EmployeeId = request.EmployeeId,
            EmployeeScheduleId =
                request.EmployeeScheduleId,
            Method = request.Method,
            Status = status,
            LateMinutes = lateMinutes
        };

        await _repository.AddAsync(attendance);

        return _mapper.Map<AttendanceDto>(
            attendance);
    }
}
