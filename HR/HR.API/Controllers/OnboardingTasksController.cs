using Asp.Versioning;
using HR.Application.Features.OnboardingTasks.Commands;
using HR.Application.Features.OnboardingTasks.Queries.GetOnboardingTasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class OnboardingTasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public OnboardingTasksController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{employeeId}")]
    public async Task<IActionResult> GetByEmployee(Guid employeeId)
    {
        var result = await _mediator.Send(new GetOnboardingTasksQuery(employeeId));
        return Ok(result);
    }

    [HttpPost("seed/{employeeId}")]
    public async Task<IActionResult> Seed(Guid employeeId)
    {
        var result = await _mediator.Send(new SeedOnboardingTasksCommand(employeeId));
        return Ok(new { success = result });
    }

    [HttpPut("complete")]
    public async Task<IActionResult> Complete(CompleteOnboardingTaskCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { success = result });
    }
}