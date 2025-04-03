using MediatR;
using AutoMapper;
using TodoSvc.Domain.Enums;
using Ardalis.GuardClauses;
using TodoSvc.Domain.Entities;
using NetHelper.Common.Models;

namespace TodoSvc.Application.Commands;

public record UpdateTodoCommentCommand : IRequest<ResultCustom<TodoItemComment>>
{
    public Guid Id { set; get; }
    public required string Comment { set; get; }

    public Guid? TodoItemId { init; get; }
}

public class UpdateTodoCommentCommandHandler(ITodoDbContext dbContext) : IRequestHandler<UpdateTodoCommentCommand, ResultCustom<TodoItemComment>>
{
    private readonly ITodoDbContext _dbContext = dbContext;

    public async Task<ResultCustom<TodoItemComment>> Handle(UpdateTodoCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // check comment exist in database 
            var entity = await _dbContext.TodoItemsComments.FindAsync([request.Id], cancellationToken);
            // return error if entiy doesn't exist
            if (entity is null) return new ResultCustom<TodoItemComment>
            {
                Status = StatusCode.NOTFOUND,
                Message = new[] { $"Error :: TodoComment Id = {request.Id} doesn't exist !" }
            };

            if (request.Comment is not null) entity.Comment = request.Comment;
            if (request.TodoItemId is not null) entity.TodoItemId = request.TodoItemId.Value;

            await _dbContext.SaveChangeAsync(cancellationToken);

            return new ResultCustom<TodoItemComment>
            {
                Status = StatusCode.OK,
                Message = new[] { $"Update TodoComment Id = {entity.Id} successfully !" }
            };
        }
        catch (Exception ex)
        {
            return new ResultCustom<TodoItemComment>
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { $"Message ::{ex}" }
            };
        }


    }
}