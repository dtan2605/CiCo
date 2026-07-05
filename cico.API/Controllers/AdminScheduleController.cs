using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cico.Application.Features.EmployeeSchedules
    .Commands.CreateEmployeeSchedule;
using cico.Application.Features.EmployeeSchedules
    .Commands.UpdateEmployeeSchedule;
using cico.Application.Features.EmployeeSchedules
    .Commands.DeleteEmployeeSchedule;
using cico.Application.Features.EmployeeSchedules
    .Commands.GenerateEmployeeSchedules;
using cico.Application.Features.EmployeeSchedules
    .Queries.GetEmployeeScheduleById;
using cico.Application.Features.EmployeeSchedules
    .Queries.GetEmployeeSchedulesPaging;
using Microsoft.AspNetCore.RateLimiting;

namespace cico.API.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/admin/schedules")]
[EnableRateLimiting("EmployeePolicy")]
public class AdminScheduleController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminScheduleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaging(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] Guid? employeeId = null,
        [FromQuery] Guid? scheduleId = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null)
    {
        var result = await _mediator.Send(
            new GetEmployeeSchedulesPagingQuery(
                pageNumber, pageSize, employeeId, scheduleId, fromDate, toDate));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetEmployeeScheduleByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [EnableRateLimiting("WritePolicy")]
    public async Task<IActionResult> Create(CreateEmployeeScheduleCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut]
    [EnableRateLimiting("WritePolicy")]
    public async Task<IActionResult> Update(UpdateEmployeeScheduleCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [EnableRateLimiting("WritePolicy")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteEmployeeScheduleCommand(id));
        return NoContent();
    }

    [HttpPost("generate")]
    [EnableRateLimiting("WritePolicy")]
    public async Task<IActionResult> Generate(
        GenerateEmployeeSchedulesCommand command)
    {
        var count = await _mediator.Send(command);
        return Ok(new { generated = count });
    }
}
