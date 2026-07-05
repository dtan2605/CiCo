using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cico.Application.Features.BiometricProfiles
    .Commands.RegisterBiometric;
using cico.Application.Features.BiometricProfiles
    .Commands.VerifyBiometric;
using cico.Application.Features.BiometricProfiles
    .Commands.DeleteBiometricProfile;
using cico.Application.Features.BiometricProfiles
    .Queries.GetBiometricProfileById;
using cico.Application.Features.BiometricProfiles
    .Queries.GetBiometricProfilesByEmployee;
using cico.Application.Features.BiometricProfiles
    .Queries.GetBiometricProfilesPaging;
using cico.Domain.Enums;

using Microsoft.AspNetCore.RateLimiting;

namespace cico.API.Controllers;

[EnableRateLimiting("EmployeePolicy")]
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BiometricProfileController
    : ControllerBase
{
    private readonly IMediator _mediator;

    public BiometricProfileController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaging(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? employeeId = null,
        [FromQuery] BiometricType? type = null,
        [FromQuery] bool? isActive = null)
    {
        var result =
            await _mediator.Send(
                new GetBiometricProfilesPagingQuery(
                    pageNumber,
                    pageSize,
                    employeeId,
                    type,
                    isActive));

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var result =
            await _mediator.Send(
                new GetBiometricProfileByIdQuery(id));

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("by-employee/{employeeId:guid}")]
    public async Task<IActionResult>
        GetByEmployee(Guid employeeId)
    {
        var result =
            await _mediator.Send(
                new GetBiometricProfilesByEmployeeQuery(
                    employeeId));

        return Ok(result);
    }

    [HttpPost("register")]
    [EnableRateLimiting("WritePolicy")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Register(
        RegisterBiometricCommand command)
    {
        var result =
            await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpPost("verify")]
    [EnableRateLimiting("WritePolicy")]
    public async Task<IActionResult> Verify(
        VerifyBiometricCommand command)
    {
        var result =
            await _mediator.Send(command);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [EnableRateLimiting("WritePolicy")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        await _mediator.Send(
            new DeleteBiometricProfileCommand(id));

        return NoContent();
    }
}
