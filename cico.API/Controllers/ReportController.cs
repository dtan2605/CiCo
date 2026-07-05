using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cico.Application.Features.Reports.Queries.GetDashboard;
using cico.Application.Features.Reports.Queries.GetDailyReport;
using cico.Application.Features.Reports.Queries.GetWeeklyReport;
using cico.Application.Features.Reports.Queries.GetMonthlyReport;

namespace cico.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(
        [FromQuery] DateOnly date)
    {
        var result = await _mediator.Send(
            new GetDashboardQuery(date));
        return Ok(result);
    }

    [HttpGet("daily/{date}")]
    public async Task<IActionResult> GetDaily(
        DateOnly date)
    {
        var result = await _mediator.Send(
            new GetDailyReportQuery(date));
        return Ok(result);
    }

    [HttpGet("weekly")]
    public async Task<IActionResult> GetWeekly(
        [FromQuery] DateOnly fromDate,
        [FromQuery] DateOnly toDate)
    {
        var result = await _mediator.Send(
            new GetWeeklyReportQuery(
                fromDate, toDate));
        return Ok(result);
    }

    [HttpGet("monthly/{year}/{month}")]
    public async Task<IActionResult> GetMonthly(
        int year, int month)
    {
        if (month < 1 || month > 12)
            return BadRequest(
                "Month must be 1-12");

        var result = await _mediator.Send(
            new GetMonthlyReportQuery(year, month));
        return Ok(result);
    }
}
