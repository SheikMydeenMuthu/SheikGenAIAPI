using Asp.Versioning;
using HR.Application.Features.LeaveRequests.Commands.ApplyLeave;
using HR.Application.Features.LeaveRequests.Commands.ApproveLeave;
using HR.Application.Features.LeaveRequests.Commands.Queries.GetLeaveRequests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class LeaveRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeaveRequestsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Apply(ApplyLeaveCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }

    [HttpPut("approve")]
    public async Task<IActionResult> Approve(ApproveLeaveCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { success = result });
    }

    [HttpGet("{employeeId}")]
    public async Task<IActionResult> GetByEmployee(Guid employeeId)
    {
        var result = await _mediator.Send(new GetLeaveRequestsQuery(employeeId));
        return Ok(result);
    }
}