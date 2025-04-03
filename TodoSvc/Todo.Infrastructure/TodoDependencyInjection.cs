using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoSvc.Application;

namespace TodoSvc.Infrastructure;

public static class TodoDependencyInjection
{
    public static IServiceCollection AddTodoInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TodoCnt");

        services.AddDbContext<TodoDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<ITodoDbContext>(provider => provider.GetRequiredService<TodoDbContext>());
        services.AddScoped<TodoDbContextInitializer>();

        return services;
    }
}