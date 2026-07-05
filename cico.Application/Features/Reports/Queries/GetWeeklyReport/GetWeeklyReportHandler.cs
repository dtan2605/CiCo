using MediatR;
using cico.Application.DTOs.Reports;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Reports.Queries.GetWeeklyReport;

public class GetWeeklyReportHandler
    : IRequestHandler<
        GetWeeklyReportQuery, WeeklyReportDto>
{
    private readonly IReportRepository _repository;

    public GetWeeklyReportHandler(
        IReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<WeeklyReportDto> Handle(
        GetWeeklyReportQuery request,
        CancellationToken cancellationToken)
    {
        var counts =
            await _repository.GetDailyCountsAsync(
                request.FromDate, request.ToDate);

        var total =
            await _repository
                .GetTotalEmployeeAsync();

        var summaries = new List<DailySummaryDto>();
        var current = request.FromDate;

        while (current <= request.ToDate)
        {
            var present = counts.GetValueOrDefault(
                current, 0);
            summaries.Add(new DailySummaryDto
            {
                Date = current,
                DayOfWeek =
                    current.DayOfWeek.ToString(),
                Present = present,
                Absent = total - present,
                Late = 0
            });
            current = current.AddDays(1);
        }

        return new WeeklyReportDto
        {
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            DailySummaries = summaries
        };
    }
}
