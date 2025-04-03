using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TodoSvc.Infrastructure;

public static class InitializerExtensions
{
    public static async Task InitializeTodoDatabaseAsync(this IServiceProvider service)
    {
        var initialiser = service.CreateScope().ServiceProvider.GetRequiredService<TodoDbContextInitializer>();

        await initialiser.InitializeAsync();
    }
}

public class TodoDbContextInitializer
{
    private readonly ILogger<TodoDbContextInitializer> _logger;
    private readonly TodoDbContext _context;

    public TodoDbContextInitializer(ILogger<TodoDbContextInitializer> logger, TodoDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitializeAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }
}
