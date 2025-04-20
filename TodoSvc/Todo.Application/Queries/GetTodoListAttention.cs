using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NetHelper.Common.Mappings;
using NetHelper.Common.Models;
using TodoSvc.Application.Dtos;

namespace TodoSvc.Application.Queries;

public record GetTodoListAttentionRequest : IRequest<PaginatedList<TodoSimple>>
{
    public Guid? ObjectId { get; init; } = null;
    public Guid? OwnerId { get; init; } = null;
    public Guid? Assigner { get; init; } = null;
    public Guid? Assignee { get; init; } = null;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetTodoListAttentionRequestHandler(ITodoDbContext dbContext, IMapper mapper) : IRequestHandler<GetTodoListAttentionRequest, PaginatedList<TodoSimple>>
{
    private readonly ITodoDbContext _db = dbContext;
    private readonly IMapper _mapper = mapper;

    public async Task<PaginatedList<TodoSimple>> Handle(GetTodoListAttentionRequest request, CancellationToken cancellationToken)
    {
        var qr = _db.TodoItems.AsQueryable();
        var today = DateTime.Today;

        qr = qr.Where(x => x.ParentTodoItemId == null);
        if (request.ObjectId is not null) { qr = qr.Where(x => x.ObjectId == request.ObjectId); }
        if (request.OwnerId is not null) { qr = qr.Where(x => x.Owner == request.OwnerId); }
        if (request.Assignee is not null) { qr = qr.Where(x => x.Assignee == request.Assignee); }
        if (request.Assigner is not null) { qr = qr.Where(x => x.Assigner == request.Assigner); }

        return await qr.Include(x => x.SubTodoItems)
          .Include(x => x.Comments)
          .Where(x => (x.Status != TodoSvc.Domain.Enums.TodoStatus.Done) && (x.CreatedDate <= today || x.DueDate <= today))
          .OrderByDescending(x => x.CreatedDate)
         .ProjectTo<TodoSimple>(_mapper.ConfigurationProvider)
         .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
