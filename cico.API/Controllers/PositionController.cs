using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cico.Application.Features.Positions.Commands.CreatePosition;
using cico.Application.Features.Positions.Commands.UpdatePosition;
using cico.Application.Features.Positions.Commands.DeletePosition;
using cico.Application.Features.Positions.Queries.GetPositionById;
using cico.Application.Features.Positions.Queries.GetPositionsPaging;

using Microsoft.AspNetCore.RateLimiting;

namespace cico.API.Controllers;

[EnableRateLimiting("PositionPolicy")]
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PositionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PositionsController(
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
                new GetPositionsPagingQuery(
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
                new GetPositionByIdQuery(id));

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [EnableRateLimiting("WritePolicy")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(
        CreatePositionCommand command)
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
        UpdatePositionCommand command)
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
            new DeletePositionCommand(id));

        return NoContent();
    }
}
