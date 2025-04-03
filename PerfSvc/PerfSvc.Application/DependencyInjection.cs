using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace PerfSvc.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPerfServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });

        return services;
    }
}
