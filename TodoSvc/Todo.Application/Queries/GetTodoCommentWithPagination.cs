using AutoMapper;
using MediatR;
using AutoMapper.QueryableExtensions;
using NetHelper.Common.Models;
using TodoSvc.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Mappings;

namespace TodoSvc.Application.Queries;

public record GetTodoCommentWithPaginationQuery : IRequest<ResultCustomPaginate<IEnumerable<TodoItemComment>>>
{
    public Guid? ParentTodoItemCommentId { set; get; }

    public Guid? TodoItemId { set; get; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetTodoCommentWithPaginationQueryHandler(ITodoDbContext dbContext, IMapper mapper) : IRequestHandler<GetTodoCommentWithPaginationQuery, ResultCustomPaginate<IEnumerable<TodoItemComment>>>
{
    private readonly ITodoDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;
    public async Task<ResultCustomPaginate<IEnumerable<TodoItemComment>>> Handle(GetTodoCommentWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            if (query.TodoItemId is null) return new ResultCustomPaginate<IEnumerable<TodoItemComment>>
            {
                Status = StatusCode.BADREQUEST,
                Message = new[] { "TodoId is required!" }
            };
            var qr = _dbContext.TodoItemsComments.AsQueryable();

            // lấy theo todoComment cha
            if (query.ParentTodoItemCommentId is not null) { qr = qr.Where(x => x.ParentTodoItemCommentId == query.ParentTodoItemCommentId); }
            // lấy theo todoId 
            if (query.TodoItemId is not null) { qr = qr.Where(x => x.TodoItemId == query.TodoItemId); }

            var totalItems = await qr.CountAsync();

            var items = await qr
                .OrderBy(x => x.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new ResultCustomPaginate<IEnumerable<TodoItemComment>>
            {
                Status = StatusCode.OK,
                Message = new[] { "Get todoComment successfully" },
                Data = items,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize),
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            return new ResultCustomPaginate<IEnumerable<TodoItemComment>>
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { $"Message ::{ex}" }
            };

        }



    }
}