namespace Notification.Infrastructure.Email;

public class SmtpSettings
{
    public string Host { get; set; } = default!;
    public int Port { get; set; }
    public string User { get; set; } = default!;
    public string AppPassword { get; set; } = default!;
    public string FromName { get; set; } = default!;
}