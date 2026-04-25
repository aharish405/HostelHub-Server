using HostelHub.Application.Common.Interfaces;
using HostelHub.Infrastructure.Identity;
using HostelHub.Infrastructure.Persistence;
using HostelHub.Infrastructure.Repositories;
using HostelHub.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HostelHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITenantService, TenantService>();
        
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var tenantService = sp.GetRequiredService<ITenantService>();
            
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();

        return services;
    }
}
