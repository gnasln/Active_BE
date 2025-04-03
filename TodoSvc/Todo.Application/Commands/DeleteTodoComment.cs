using MediatR;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Models;
using TodoSvc.Domain.Entities;
using System.Reflection.Metadata.Ecma335;

namespace TodoSvc.Application.Commands;

public record DeleteTodoCommentCommand : IRequest<ResultCustom<TodoItemComment>>
{
    public Guid Id { set; get; }
}

public class DeleteTodoCommentCommandHandler(ITodoDbContext dbContext) : IRequestHandler<DeleteTodoCommentCommand, ResultCustom<TodoItemComment>>
{
    private readonly ITodoDbContext _dbContext = dbContext;

    public async Task<ResultCustom<TodoItemComment>> Handle(DeleteTodoCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _dbContext.TodoItemsComments.FindAsync([request.Id], cancellationToken);
            if (entity is null)
            {
                return new ResultCustom<TodoItemComment>
                {
                    Status = StatusCode.NOTFOUND,
                    Message = new[] { $"Error :: TodoComment Id = {request.Id} doesn't exist !" }
                };
            }

            _dbContext.TodoItemsComments.Remove(entity);
            await _dbContext.SaveChangeAsync(cancellationToken);

            return new ResultCustom<TodoItemComment>
            {
                Status = StatusCode.OK,
                Message = new[] { $"Message :: Delete TodoComment Id = {request.Id} successfully !"}
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