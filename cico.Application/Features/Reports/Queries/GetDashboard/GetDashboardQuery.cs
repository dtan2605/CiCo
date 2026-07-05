using MediatR;
using cico.Application.DTOs.Reports;

namespace cico.Application.Features.Reports.Queries.GetDashboard;

public record GetDashboardQuery(
    DateOnly Date
) : IRequest<DashboardDto>;
