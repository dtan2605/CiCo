using AutoMapper;
using MediatR;
using cico.Application.DTOs.Reports;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Reports.Queries.GetDailyReport;

public class GetDailyReportHandler
    : IRequestHandler<
        GetDailyReportQuery, DailyReportDto>
{
    private readonly IReportRepository _repository;
    private readonly IMapper _mapper;

    public GetDailyReportHandler(
        IReportRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<DailyReportDto> Handle(
        GetDailyReportQuery request,
        CancellationToken cancellationToken)
    {
        var total =
            await _repository
                .GetTotalEmployeeAsync();

        var present =
            await _repository
                .GetPresentEmployeeAsync(
                    request.Date);

        var absent =
            await _repository
                .GetAbsentEmployeeAsync(
                    request.Date);

        var late =
            await _repository
                .GetLateEmployeeAsync(
                    request.Date);

        var records =
            await _repository
                .GetAttendanceByDateAsync(
                    request.Date);

        return new DailyReportDto
        {
            Date = request.Date,
            TotalEmployees = total,
            Present = present,
            Absent = absent,
            Late = late,
            Attendances =
                _mapper.Map<
                    List<AttendanceBriefDto>>(
                    records)
        };
    }
}
