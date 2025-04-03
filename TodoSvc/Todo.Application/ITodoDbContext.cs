using Microsoft.EntityFrameworkCore;
using Todo.Domain.Entities;
using TodoSvc.Domain.Entities;

namespace TodoSvc.Application;

public interface ITodoDbContext
{
    DbSet<TodoItem> TodoItems { get; }
    DbSet<TodoItemComment> TodoItemsComments { get; }

    DbSet<DailyTask> DailyTasks { get; }

    DbSet<TodoHistory> TodoHistories { get; }
    DbSet<TodoMembers> TodoMembers { get; }
    Task<int> SaveChangeAsync(CancellationToken cancellationToken);
}
