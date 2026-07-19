using MediatR;
using Notification.Application.Interfaces;
using Notification.Infrastructure.Email;
using Notification.Infrastructure.ServiceBus;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.Configure<ServiceBusSettings>(builder.Configuration.GetSection("ServiceBusSettings"));

builder.Services.AddTransient<DeadLetterProcessor>();
builder.Services.AddSingleton<IEmailService, SmtpEmailService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
    typeof(Notification.Application.Features.SendLeaveEmail.SendLeaveEmailCommand).Assembly));

builder.Services.AddHostedService<LeaveEventConsumer>();

var host = builder.Build();

//Remove from Dead log Queue messages.
// using (var scope = host.Services.CreateScope())
// {
//     var dlqProcessor = scope.ServiceProvider.GetRequiredService<DeadLetterProcessor>();
//     await dlqProcessor.ReadAndClearAsync();
// }
host.Run();