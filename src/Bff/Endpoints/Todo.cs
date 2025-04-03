using Bff.Application.Sample.Queries;
using Bff.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using NetHelper.Common.Models;
using ServiceStack.Host;
using TodoSvc.Application.Commands;
using TodoSvc.Application.Dtos;
using TodoSvc.Application.Queries;
using TodoSvc.Domain.Entities;
using Todo.Application.Queries;
using Todo.Domain.Entities;
using Todo.Application.Dtos;
namespace Bff.Endpoints;

public class Todo : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(GetTodoListByUnit, "/list")
            .MapGet(GetTodoDetail, "/detail")
            .MapPost(CreateTodoItem, "/create")
            .MapDelete(DeleteTodoItem, "/delete")
            .MapPost(GetTodoHistory, "/edit-history")
            .MapPut(UpdateTodoItem, "/update")
            .MapPatch(ChangeParent, "/change_parent")
            .MapPatch(ChangeUnitId, "/change-unit-id")
            // rad2
            .MapPost(GetTodoListAttention, "/list-attention")
            .MapPost(CreateTodoItemComment, "/comment/create")
            .MapPatch(UpdateTodoItemComment, "/comment/update")
            .MapDelete(DeleteTodoComment, "/comment/delete/{Id}")
            .MapGet(GetCommentByTodo, "/comment")
            //daily task
            .MapGet(GetListTodoFromDailyTask, "/get-list-todo-from-daily-task");
        app.MapGroup(this)
            .RequireAuthorization("admin")
            .MapGet(GetDailyTaskByAdmin, "/get-daily-task-by-admin");

        ;
    }
    public async Task<PaginatedList<TodoSimple>> GetTodoListByUnit([FromServices] ISender sender, [FromBody] GetTodoWithPaginationRequest query)
    {
        return await sender.Send(query);
    }

    public async Task<TodoDetail?> GetTodoDetail([FromServices] ISender sender, [FromQuery] Guid rq)
    {
        var query = new GetTodoDetailRequest() { Id = rq };
        return await sender.Send(query);
    }

    public async Task<TodoMemberDto> CreateTodoItem([FromServices] ISender sender, [FromBody] CreateTodoCommand command)
    {
        return await sender.Send(command);
    }

    public async Task DeleteTodoItem([FromServices] ISender sender, [FromBody] DeleteTodoCommand command)
    {
        await sender.Send(command);
    }

    public async Task<PaginatedList<TodoHistorysDTO>> GetTodoHistory([FromServices] ISender sender, [FromBody] GetTodoHistoryRequest query)
    {
        return await sender.Send(query);
    }



    public async Task UpdateTodoItem([FromServices] ISender sender, [FromBody] UpdateTodoRequest command)
    {
        await sender.Send(command);
    }

    public async Task ChangeParent(ISender sender, ChangeParrentTodoCommand command)
    {
        await sender.Send(command);
    }

    public async Task ChangeUnitId(ISender sender, ChangeUnitIdTodoCommand command)
    {
        await sender.Send(command);
    }

    // rad2 
    public async Task<PaginatedList<TodoSimple>> GetTodoListAttention(ISender sender, [FromBody] GetTodoListAttentionRequest query)
    {
        return await sender.Send(query);
    }

    public async Task<ResultCustom<TodoItemComment>> CreateTodoItemComment(ISender sender, [FromBody] CreateTodoItemsCommentCommand command)
    {
        return await sender.Send(command);
    }

    public async Task<ResultCustom<TodoItemComment>> UpdateTodoItemComment(ISender sender, [FromBody] UpdateTodoCommentCommand command)
    {
        return await sender.Send(command);
    }

    public async Task<ResultCustom<TodoItemComment>> DeleteTodoComment(ISender sender, [FromRoute] Guid Id)
    {
        return await sender.Send(new DeleteTodoCommentCommand() { Id = Id });
    }

    public async Task<ResultCustomPaginate<IEnumerable<TodoItemComment>>> GetCommentByTodo(
        ISender sender,
        [FromQuery] Guid? todoid,
        [FromQuery] Guid? parentcommentid,
        [FromQuery] int pagenumber,
        [FromQuery] int pagesize
    )
    {
        return await sender.Send(new GetTodoCommentWithPaginationQuery()
        {
            TodoItemId = todoid,
            ParentTodoItemCommentId = parentcommentid,
            PageNumber = pagenumber,
            PageSize = pagesize
        });
    }

    public async Task<ResultCustomPaginate<IEnumerable<DailyTaskDto>>> GetListTodoFromDailyTask(ISender sender, int page, int pageSize)
    {
        return await sender.Send(new GetTodoListFromDailyTask() { PageNumber = page, PageSize = pageSize });
    }

    public async Task<ResultCustomPaginate<IEnumerable<DailyTaskDto>>> GetDailyTaskByAdmin(ISender sender, int page, int pageSize)
    {
        return await sender.Send(new GetTodoListFromDailyTaskByAdmin() { PageNumber = page, PageSize = pageSize });
    }

}

