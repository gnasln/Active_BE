using AutoMapper;
using MediatR;
using NetHelper.Common.Models;
using TodoSvc.Application.Dtos;
using TodoSvc.Domain.Entities;
using TodoSvc.Domain.Enums;

namespace TodoSvc.Application.Commands;

public record CreateTodoItemsCommentCommand : IRequest<ResultCustom<TodoItemComment>>
{
    public required string Comment { get; set; }

    public Guid? ParentTodoItemCommentId { get; set; }

    public Guid TodoItemId { set; get; }

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateTodoItemsCommentCommand, TodoItemComment>();
        }
    }

}

public class CreateTodoItemsCommentCommandHandler(ITodoDbContext dbContext, IMapper mapper) : IRequestHandler<CreateTodoItemsCommentCommand, ResultCustom<TodoItemComment>>
{

    private readonly ITodoDbContext _db = dbContext;
    private readonly IMapper _mapper = mapper;

    public async Task<ResultCustom<TodoItemComment>> Handle(CreateTodoItemsCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<CreateTodoItemsCommentCommand, TodoItemComment>(request);
            await _db.TodoItemsComments.AddAsync(entity, cancellationToken);
            await _db.SaveChangeAsync(cancellationToken);

            return new ResultCustom<TodoItemComment>
            {
                Status = StatusCode.CREATED,
                Message = new[] { $"Created !" },
                Data = entity
            };
        }
        catch (Exception ex)
        {
            return new ResultCustom<TodoItemComment>
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { $"Error :: {ex}" }
            };
        }
    }
}