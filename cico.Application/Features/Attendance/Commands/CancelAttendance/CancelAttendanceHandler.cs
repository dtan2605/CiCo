using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Attendance.Commands.CancelAttendance;

public class CancelAttendanceHandler : IRequestHandler<CancelAttendanceCommand>
{
    private const string RequiredCode = "270904";

    private readonly IAttendanceRepository _repository;

    public CancelAttendanceHandler(IAttendanceRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(CancelAttendanceCommand request, CancellationToken cancellationToken)
    {
        if (request.SecurityCode != RequiredCode)
            throw new DomainException("Invalid security code");

        var attendance = await _repository.GetByIdAsync(request.AttendanceId);
        if (attendance == null)
            throw new DomainException("Attendance record not found");

        await _repository.DeleteAsync(attendance);
    }
}
