using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Todo.Domain.Entities;
using TodoSvc.Application;
using TodoSvc.Application.Dtos;
using TodoSvc.Domain.Entities;

namespace TodoSvc.Infrastructure;

public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options), ITodoDbContext
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<TodoItemComment> TodoItemsComments => Set<TodoItemComment>();
    public DbSet<DailyTask> DailyTasks => Set<DailyTask>();
    public DbSet<TodoHistory> TodoHistories => Set<TodoHistory>();
    public DbSet<TodoMembers> TodoMembers => Set<TodoMembers>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    Task<int> ITodoDbContext.SaveChangeAsync(CancellationToken cancellationToken)
    {
        return base.SaveChangesAsync();
    }
}
