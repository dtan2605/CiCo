using MediatR;
using cico.Application.DTOs.Reports;

namespace cico.Application.Features.Reports.Queries.GetDailyReport;

public record GetDailyReportQuery(
    DateOnly Date
) : IRequest<DailyReportDto>;
