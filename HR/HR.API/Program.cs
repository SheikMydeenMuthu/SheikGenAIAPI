using System.Text;
using Asp.Versioning;
using HR.Application;
using HR.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Diagnostics;
using HR.Application.Common.Exceptions;
using Serilog;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.EntityFrameworkCore;
using HR.Infrastructure.Messaging;
using HR.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ServiceBusSettings>(builder.Configuration.GetSection("ServiceBusSettings"));
builder.Services.AddSingleton<IEventPublisher, ServiceBusPublisher>();

// ---------- Serilog ----------
var useAppInsights = builder.Configuration.GetValue<bool>("Observability:UseApplicationInsights");
var aiConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];

var loggerConfig = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/hrapi-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14);

if (useAppInsights && !string.IsNullOrWhiteSpace(aiConnectionString))
{
    var telemetryConfig = new Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration
    {
        ConnectionString = aiConnectionString
    };

    loggerConfig.WriteTo.ApplicationInsights(
        telemetryConfig,
        new Serilog.Sinks.ApplicationInsights.TelemetryConverters.TraceTelemetryConverter());
}

Log.Logger = loggerConfig.CreateLogger();
builder.Host.UseSerilog();

// ---------- OpenTelemetry ----------
var otelBuilder = builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("HR.API"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation();

        if (useAppInsights && !string.IsNullOrWhiteSpace(aiConnectionString))
        {
            tracing.AddAzureMonitorTraceExporter(o => o.ConnectionString = aiConnectionString);
        }
        else
        {
            tracing.AddConsoleExporter();
        }
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "HR.API", Version = "v1" });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler(errApp =>
{
    errApp.Run(async context =>
    {
        var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = ex is NotFoundException ? 404 : 500;
        await context.Response.WriteAsJsonAsync(new { error = ex?.Message });
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();