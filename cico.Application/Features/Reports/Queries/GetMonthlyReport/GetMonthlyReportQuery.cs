using MediatR;
using cico.Application.DTOs.Reports;

namespace cico.Application.Features.Reports.Queries.GetMonthlyReport;

public record GetMonthlyReportQuery(
    int Year,
    int Month
) : IRequest<MonthlyReportDto>;
