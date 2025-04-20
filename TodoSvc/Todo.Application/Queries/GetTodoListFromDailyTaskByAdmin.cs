using Bff.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Models;
using PerfSvc.Infrastructure;
using TodoSvc.Application.Dtos;

namespace TodoSvc.Application.Queries;

public class GetTodoListFromDailyTaskByAdmin : IRequest<ResultCustomPaginate<IEnumerable<DailyTaskDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetTodoListFromDailyTaskByAdminHandler : IRequestHandler<GetTodoListFromDailyTaskByAdmin, ResultCustomPaginate<IEnumerable<DailyTaskDto>>>
{
    private readonly ITodoDbContext _context;
    private readonly IPerfDbContext _perfContext;
    private readonly IApplicationDbContext _tenantContext;
    public GetTodoListFromDailyTaskByAdminHandler(ITodoDbContext context, IApplicationDbContext tenantContext, IPerfDbContext perfDbContext)
    {
        _context = context;
        _tenantContext = tenantContext;
        _perfContext = perfDbContext;
    }
    public async Task<ResultCustomPaginate<IEnumerable<DailyTaskDto>>> Handle(GetTodoListFromDailyTaskByAdmin request, CancellationToken cancellationToken)
    {
        try
        {
            // Load TodoItems
            var todoItems = await _context.TodoItems
                .Where(t => t.DueDate == DateTime.Today)
                .OrderByDescending(t => t.CreatedDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Load Units
            var unitIds = todoItems.Select(t => t.ObjectId).Distinct();
            var units = await _perfContext.Units
                .Where(u => unitIds.Contains(u.Id))
                .ToListAsync(cancellationToken);

            // Load Tenants
            var tenantIds = units.Select(u => u.TenantId).Distinct();
            var tenants = await _tenantContext.Tenants
                .Where(t => tenantIds.Contains(t.Id))
                .ToListAsync(cancellationToken);

            // Combine results
            var listDailyTask = todoItems.Select(todo => {
                var unit = units.FirstOrDefault(u => u.Id == todo.ObjectId);
                var tenant = tenants.FirstOrDefault(t => t.Id == unit?.TenantId);
                return new DailyTaskDto
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    CreatedDate = todo.CreatedDate,
                    DueDate = todo.DueDate,
                    Priority = todo.Priority,
                    TenantId = unit?.TenantId,
                    TenantName = tenant?.Name,
                    TodoOwnerId = todo.Owner,
                    TodoOwnerName = todo.OwnerName,
                    IsDone = todo.IsDone
                };
            }).ToList();

            return new ResultCustomPaginate<IEnumerable<DailyTaskDto>>
            {
                Status = StatusCode.OK,
                Message = new[] { "Get list todo from daily task success" },
                Data = listDailyTask,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = await _context.TodoItems.CountAsync(t => t.DueDate == DateTime.Today, cancellationToken)
            };
        }
        catch (Exception ex)
        {
            return new ResultCustomPaginate<IEnumerable<DailyTaskDto>>
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { ex.Message }
            };
        }
    }

}