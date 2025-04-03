using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace TodoSvc.Application.Commands;

public record DeleteTodoCommand(Guid Id) : IRequest;

public class DeleteTodoCommandHandler(ITodoDbContext dbContext) : IRequestHandler<DeleteTodoCommand>
{
    private readonly ITodoDbContext _dbContext = dbContext;

    public async Task Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.TodoItems
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        var todoMembers = await _dbContext.TodoMembers.Where(t => t.TodoId == request.Id).ToListAsync();
        if (todoMembers.Any())
        {
            _dbContext.TodoMembers.RemoveRange(todoMembers);
        }
        _dbContext.TodoItems.Remove(entity);

        await _dbContext.SaveChangeAsync(cancellationToken);
    }
}