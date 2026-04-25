using FluentValidation;
using HostelHub.Application.Common.Behaviors;
using HostelHub.Application.Common.Mappings;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HostelHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Add AutoMapper inside Di
        services.AddAutoMapper(cfg => {
            cfg.AddProfile<HostelProfile>();
        });
        
        return services;
    }
}
