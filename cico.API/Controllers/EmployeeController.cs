using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cico.Application.Features.Employees.Commands.CreateEmployee;
using cico.Application.Features.Employees.Commands.UpdateEmployee;
using cico.Application.Features.Employees.Commands.DeleteEmployee;
using cico.Application.Features.Employees.Queries.GetEmployeeById;
using cico.Application.Features.Employees.Queries.GetEmployeeByUserId;
using cico.Application.Features.Employees.Queries.GetEmployeesPaging;

using Microsoft.AspNetCore.RateLimiting;

namespace cico.API.Controllers;

[EnableRateLimiting("EmployeePolicy")]
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeesController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [EnableRateLimiting("WritePolicy")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> GetPaging(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null)
    {
        var result =
            await _mediator.Send(
                new GetEmployeesPagingQuery(
                    pageNumber,
                    pageSize,
                    keyword));

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var result =
            await _mediator.Send(
                new GetEmployeeByIdQuery(id));

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("by-user/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetByUserId(
        Guid userId)
    {
        var result =
            await _mediator.Send(
                new GetEmployeeByUserIdQuery(userId));

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [EnableRateLimiting("WritePolicy")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(
        CreateEmployeeCommand command)
    {
        var id =
            await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            id);
    }

    [HttpPut]
    [EnableRateLimiting("WritePolicy")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Update(
        UpdateEmployeeCommand command)
    {
        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [EnableRateLimiting("DeletePolicy")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        await _mediator.Send(
            new DeleteEmployeeCommand(id));

        return NoContent();
    }
}