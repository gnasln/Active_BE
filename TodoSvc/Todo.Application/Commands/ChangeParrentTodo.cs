using Ardalis.GuardClauses;
using MediatR;

namespace TodoSvc.Application.Commands;

public record ChangeParrentTodoCommand : IRequest<bool>
{
    public Guid Id { get; init; }
    public Guid ParentId { get; init; }
}

public class ChangeParrentTodoCommandHandler(ITodoDbContext context) : IRequestHandler<ChangeParrentTodoCommand, bool>
{
    private readonly ITodoDbContext _context = context;
    public async Task<bool> Handle(ChangeParrentTodoCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems.FindAsync([request.Id], cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.ParentTodoItemId = request.ParentId;

        var r = await _context.SaveChangeAsync(cancellationToken);
        return r > 0;
    }
}