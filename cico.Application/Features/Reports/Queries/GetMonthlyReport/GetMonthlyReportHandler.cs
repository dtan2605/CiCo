using MediatR;
using cico.Application.DTOs.Reports;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Reports.Queries.GetMonthlyReport;

public class GetMonthlyReportHandler
    : IRequestHandler<
        GetMonthlyReportQuery, MonthlyReportDto>
{
    private readonly IReportRepository _repository;

    public GetMonthlyReportHandler(
        IReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<MonthlyReportDto> Handle(
        GetMonthlyReportQuery request,
        CancellationToken cancellationToken)
    {
        var from = new DateOnly(
            request.Year, request.Month, 1);
        var to = from.AddMonths(1).AddDays(-1);

        var counts =
            await _repository.GetDailyCountsAsync(
                from, to);

        var total =
            await _repository
                .GetTotalEmployeeAsync();

        var lateCounts =
            await _repository.GetDailyLateCountsAsync(
                from, to);

        var summaries = new List<DailySummaryDto>();
        var totalPresent = 0;
        var totalLate = 0;
        var current = from;

        while (current <= to)
        {
            var present = counts.GetValueOrDefault(
                current, 0);
            var late = lateCounts.GetValueOrDefault(
                current, 0);
            totalPresent += present;
            totalLate += late;
            summaries.Add(new DailySummaryDto
            {
                Date = current,
                DayOfWeek =
                    current.DayOfWeek.ToString(),
                Present = present,
                Absent = total - present,
                Late = late
            });
            current = current.AddDays(1);
        }

        var workingDays = summaries.Count;
        var attendanceRate = workingDays > 0
            ? Math.Round(
                (double)totalPresent /
                (total * workingDays) * 100, 2)
            : 0;

        return new MonthlyReportDto
        {
            Year = request.Year,
            Month = request.Month,
            TotalEmployees = total,
            DailySummaries = summaries,
            Summary = new MonthlySummaryDto
            {
                TotalWorkingDays = workingDays,
                TotalPresent = totalPresent,
                TotalAbsent =
                    total * workingDays -
                    totalPresent,
                TotalLate = totalLate,
                AttendanceRate = attendanceRate
            }
        };
    }
}
