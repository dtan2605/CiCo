using MediatR;
using cico.Application.DTOs.Reports;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Reports.Queries.GetDashboard;

public class GetDashboardHandler
    : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IReportRepository _repository;

    public GetDashboardHandler(
        IReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<DashboardDto> Handle(
        GetDashboardQuery request,
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

        return new DashboardDto
        {
            TotalEmployees = total,
            PresentToday = present,
            AbsentToday = absent
        };
    }
}
