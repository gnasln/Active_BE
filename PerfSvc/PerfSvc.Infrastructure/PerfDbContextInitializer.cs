using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PerfSvc.Infrastructure;

public static class InitializerExtensions
{
    public static async Task InitializePerfDatabaseAsync(this IServiceProvider service)
    {
        var initializer = service.CreateScope().ServiceProvider.GetRequiredService<PerfDbContextInitializer>();

        await initializer.InitializeAsync();
    }
}

public class PerfDbContextInitializer
{
    private readonly ILogger<PerfDbContextInitializer> _logger;
    private readonly PerfDbContext _context;

    public PerfDbContextInitializer(ILogger<PerfDbContextInitializer> logger, PerfDbContext context)
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
