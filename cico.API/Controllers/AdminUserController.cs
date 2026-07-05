using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cico.Application.Features.Admin.Queries;
using cico.Application.Features.Admin.Commands;
using cico.Application.Features.Users.Queries.GetUserById;
using cico.Application.Features.Users.Queries.GetUsersPaging;
using cico.Application.Features.Users.Commands.DeleteUser;
using Microsoft.AspNetCore.RateLimiting;

namespace cico.API.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/admin")]
[EnableRateLimiting("EmployeePolicy")]
public class AdminUserController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminUserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null)
    {
        var result = await _mediator.Send(
            new GetUsersPagingQuery(pageNumber, pageSize, keyword));
        return Ok(result);
    }

    [HttpGet("users/{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost("users")]
    [EnableRateLimiting("WritePolicy")]
    public async Task<IActionResult> CreateUser(CreateAdminUserCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUser), new { id }, id);
    }

    [HttpPut("users/{id:guid}")]
    [EnableRateLimiting("WritePolicy")]
    public async Task<IActionResult> UpdateUser(Guid id, UpdateAdminUserCommand command)
    {
        if (id != command.Id)
            return BadRequest("Id mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("users/{id:guid}")]
    [EnableRateLimiting("DeletePolicy")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        var result = await _mediator.Send(new GetRolesQuery());
        return Ok(result);
    }
}
