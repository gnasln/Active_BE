using Microsoft.AspNetCore.Mvc;
using PerfSvc.Application.KeyResult.Commands;
using PerfSvc.Application.KeyResult.Queries;
using PerfSvc.Application.KeyResultMember.Commands;
using PerfSvc.Application.KeyResultMember.Queries;

namespace Bff.Endpoints;

public class KeyResult : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
       app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateKeyResult, "/create")
            .MapPatch(UpdatekeyResult, "/update")
            .MapGet(GetMemberFromKeyResult, "/get-member-from-key-result/{Id}")
            .MapPost(AddMemberToKeyResult, "/add-member")
            .MapDelete(DeleteKeyResult, "/delete/{Id}")
            .MapDelete(DeleteKeyResultMember, "/delete-member/{Id}")
            .MapPost(GetAllKeyResultFromObject, "/get-all-KeyResult-from-object")
            .MapGet(GetKeyResultById, "/keyResult-detail/{Id}")
            .MapPost(GetAllKRHistory, "/get-kr-history")
            ;
    }
    
    public async Task<IResult> CreateKeyResult(ISender sender, [FromBody] CreateKeyResultCommand command)
    {
        var res = await sender.Send(command);
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }
    public async Task<IResult> AddMemberToKeyResult(ISender sender, [FromBody] AddMemberToKeyResultCommand command)
    {
        var res = await sender.Send(command);
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }
    public async Task<IResult> UpdatekeyResult(ISender sender, [FromBody] UpdateKeyResultCommand command)
    {
        var res = await sender.Send(command);
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }
    public async Task<IResult> GetMemberFromKeyResult(ISender sender, [FromRoute] Guid Id)
    {
        var res = await sender.Send(new GetMemberFromKeyResultQuery { KeyResultId = Id });
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }
    public async Task<IResult> DeleteKeyResult(ISender sender, [FromRoute] Guid Id)
    {
        var res = await sender.Send(new DeleteKeyResultCommand { KeyResultId = Id });
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }
    public async Task<IResult> DeleteKeyResultMember(ISender sender, [FromRoute] Guid Id)
    {
        var res = await sender.Send(new DeleteMemberFromKeyResultCommand { Id = Id });
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }
    public async Task<IResult> GetAllKeyResultFromObject(ISender sender, [FromBody]GetAllKeyResultQuery query)
    {
        var res = await sender.Send(query);
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }
    public async Task<IResult> GetKeyResultById(ISender sender, [FromRoute] Guid Id)
    {
        var res = await sender.Send(new GetKeyResultByIdQuery { KeyResultId = Id });
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }
    public async Task<IResult> GetAllKRHistory(ISender sender, [FromBody] GetAllKeyResultHisroryRequest command) {
        var res = await sender.Send(command);
        return Results.Ok(new
        {
           message = "success",
           data = res
        });
    }
}