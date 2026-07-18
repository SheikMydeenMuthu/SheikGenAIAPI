using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Auth.Application.Common.Behaviors;
using Auth.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using Auth.API.Filters;
using Serilog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Azure.Monitor.OpenTelemetry.Exporter;

var builder = WebApplication.CreateBuilder(args);

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
        path: "logs/authapi-.txt",
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
    .ConfigureResource(resource => resource.AddService("Auth.API"))
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

// Infrastructure (DbContext, Repos, UoW, JWT service)
builder.Services.AddInfrastructure(builder.Configuration);

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Auth.Application.Features.Auth.Commands.LoginCommand).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Auth.Application.Features.Auth.Commands.LoginCommand).Assembly);
builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Controllers
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<LoggingActionFilter>();
    options.Filters.Add<AdminOnlyFilter>();
    options.Filters.Add<ApiResponseFilter>();
});

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = Asp.Versioning.ApiVersionReader.Combine(
        new Asp.Versioning.UrlSegmentApiVersionReader(),
        new Asp.Versioning.HeaderApiVersionReader("X-Api-Version"));
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// JWT Authentication
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

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();