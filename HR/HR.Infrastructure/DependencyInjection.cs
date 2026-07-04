using HR.Application.Interfaces;
using HR.Infrastructure.Persistence;
using HR.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HR.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<HRDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("HRDbConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
        services.AddScoped<IOnboardingTaskRepository, OnboardingTaskRepository>();
        
        return services;
    }
}