using Ardalis.GuardClauses;
using MediatR;

namespace TodoSvc.Application.Commands
{
    //public class ChangeUnitIdTodoCommand : IRequest<bool>
    //{
    //    public Guid Id { get; init; }
    //    public Guid UnitId { get; init; }
    //}

    //public class ChangeUnitIdTodoCommandHandler : IRequestHandler<ChangeUnitIdTodoCommand, bool>
    //{
    //    private readonly ITodoDbContext _context;
    //    public ChangeUnitIdTodoCommandHandler(ITodoDbContext context)
    //    {
    //        _context = context;
    //    }

    //    public async Task<bool> Handle(ChangeUnitIdTodoCommand request, CancellationToken cancellationToken)
    //    {
    //        var entity = await _context.TodoItems.FindAsync([request.Id], cancellationToken);
    //        Guard.Against.NotFound(request.Id, entity);


    //        if (entity.UnitId == Guid.Empty)
    //        {
    //            entity.UnitId = request.UnitId;
    //            if (entity.SubTodoItems is not null)
    //                foreach (var subTodoItem in entity.SubTodoItems)
    //                {
    //                    subTodoItem.UnitId = request.UnitId;
    //                }

    //            var r = await _context.SaveChangeAsync(cancellationToken);
    //            return r > 0;
    //        }
    //        return false;
    //    }
    //}

    public record ChangeUnitIdTodoCommand : IRequest<bool>
    {
        public Guid Id { get; init; }
        public Guid UnitId { get; init; }
    }

    public class ChangeUnitIdTodoCommandHandler : IRequestHandler<ChangeUnitIdTodoCommand, bool>
    {
        private readonly ITodoDbContext _context;
        public ChangeUnitIdTodoCommandHandler(ITodoDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ChangeUnitIdTodoCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.TodoItems.FindAsync(new object[] { request.Id }, cancellationToken);
            Guard.Against.NotFound(request.Id, entity);

            entity.ObjectId = request.UnitId;

            if (entity.SubTodoItems != null)
            {
                foreach (var subTodoItem in entity.SubTodoItems)
                {
                    subTodoItem.ObjectId = request.UnitId;
                }
            }

            var result = await _context.SaveChangeAsync(cancellationToken);
            return result > 0;
        }
    }
}