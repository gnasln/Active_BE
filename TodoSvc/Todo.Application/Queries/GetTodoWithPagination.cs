using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Mappings;
using NetHelper.Common.Models;
using TodoSvc.Application.Dtos;

namespace TodoSvc.Application.Queries;

public record GetTodoWithPaginationRequest : IRequest<PaginatedList<TodoSimple>>
{
    public Guid? UnitId { get; init; } = null;
    public Guid? OwnerId { get; init; } = null;
    public Guid? Assigner { get; init; } = null;
    public Guid? Assignee { get; init; } = null;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
};

public class GetTodoWithPaginationRequestHandler(ITodoDbContext db, IMapper mapper) : IRequestHandler<GetTodoWithPaginationRequest, PaginatedList<TodoSimple>>
{
    private readonly ITodoDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    public async Task<PaginatedList<TodoSimple>> Handle(GetTodoWithPaginationRequest request, CancellationToken cancellationToken)
    {
        var qr = _db.TodoItems.AsQueryable();
        qr = qr.Where(x => x.ParentTodoItemId == null);
        if (request.UnitId is not null) { qr = qr.Where(x => x.UnitId == request.UnitId); }
        if (request.OwnerId is not null) { qr = qr.Where(x => x.Owner == request.OwnerId); }
        if (request.Assignee is not null) { qr = qr.Where(x => x.Assignee == request.Assignee); }
        if (request.Assigner is not null) { qr = qr.Where(x => x.Assigner == request.Assigner); }
        return await qr.Include(x => x.SubTodoItems).OrderByDescending(x => x.CreatedDate)
           .ProjectTo<TodoSimple>(_mapper.ConfigurationProvider)
           .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
