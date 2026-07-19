using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Application.Interfaces;

namespace Notification.Application.Features.SendLeaveEmail;

public class SendLeaveEmailHandler : IRequestHandler<SendLeaveEmailCommand, Unit>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<SendLeaveEmailHandler> _logger;

    public SendLeaveEmailHandler(IEmailService emailService, ILogger<SendLeaveEmailHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Unit> Handle(SendLeaveEmailCommand request, CancellationToken cancellationToken)
    {
        var evt = request.Event;
        var subject = $"Leave Request - {evt.EmployeeName}";
        var body = $"""
            <p>Employee <b>{evt.EmployeeName}</b> has applied for <b>{evt.LeaveType}</b> leave.</p>
            <p>From: {evt.FromDate:dd-MMM-yyyy}<br/>To: {evt.ToDate:dd-MMM-yyyy}</p>
            <p>Please review and approve.</p>
            """;

        await _emailService.SendEmailAsync(evt.ManagerEmail, subject, body, cancellationToken);
        _logger.LogInformation("Leave email sent for EmployeeId {EmployeeId} to {ManagerEmail}", evt.EmployeeId, evt.ManagerEmail);

        return Unit.Value;
    }
}