using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cico.Application.Features.Departments.Commands.CreateDepartment;
using cico.Application.Features.Departments.Commands.UpdateDepartment;
using cico.Application.Features.Departments.Commands.DeleteDepartment;
using cico.Application.Features.Departments.Queries.GetDepartmentById;
using cico.Application.Features.Departments.Queries.GetDepartmentsPaging;

using Microsoft.AspNetCore.RateLimiting;

namespace cico.API.Controllers;

[EnableRateLimiting("EmployeePolicy")]
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DepartmentsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaging(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null)
    {
        var result =
            await _mediator.Send(
                new GetDepartmentsPagingQuery(
                    pageNumber,
                    pageSize,
                    keyword));

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var result =
            await _mediator.Send(
                new GetDepartmentByIdQuery(id));

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [EnableRateLimiting("WritePolicy")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Create(
        CreateDepartmentCommand command)
    {
        var result =
            await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpPut]
    [EnableRateLimiting("WritePolicy")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Update(
        UpdateDepartmentCommand command)
    {
        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [EnableRateLimiting("WritePolicy")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        await _mediator.Send(
            new DeleteDepartmentCommand(id));

        return NoContent();
    }
}
