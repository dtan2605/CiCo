using MediatR;
using cico.Application.DTOs.Reports;

namespace cico.Application.Features.Reports.Queries.GetWeeklyReport;

public record GetWeeklyReportQuery(
    DateOnly FromDate,
    DateOnly ToDate
) : IRequest<WeeklyReportDto>;
