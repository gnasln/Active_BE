using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PerfSvc.Infrastructure.Interface.Repository;
using PerfSvc.Infrastructure.Persistence.Repository;

namespace PerfSvc.Infrastructure;

public static class PerfDependencyInjection
{
    public static IServiceCollection AddPerfInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PerfCnt");

        services.AddDbContext<PerfDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IPerfDbContext>(provider => provider.GetRequiredService<PerfDbContext>());
        services.AddScoped<PerfDbContextInitializer>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<IUnitMemberRepository, UnitMemberRepository>();
        services.AddScoped<IObjectRepository, ObjectRepository>();
        services.AddScoped<IKeyResultRepository, KeyResultRepository>();
        services.AddScoped<IObjectMemberRepository, ObjectMemberRepository>();
        services.AddScoped<IKeyResultMemberRepository, KeyResultMemberRepository>();

        return services;
    }
}