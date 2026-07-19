using MediatR;
using Notification.Application.DTOs;

namespace Notification.Application.Features.SendLeaveEmail;

public class SendLeaveEmailCommand : IRequest<Unit>
{
    public LeaveRequestedEvent Event { get; set; } = default!;
}