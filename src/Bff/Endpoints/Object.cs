using Microsoft.AspNetCore.Mvc;
using PerfSvc.Application.KeyResult.Queries;
using PerfSvc.Application.Object.Commands;
using PerfSvc.Application.Object.Queries;
using PerfSvc.Application.ObjectMember.Commands;
using PerfSvc.Application.ObjectMember.Queries;

namespace Bff.Endpoints;

public class Object : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
           .RequireAuthorization()
            .MapPost(CreateObject, "/create")
            .MapPost(AddMemberToObject, "/add-member")
            .MapPatch(UpdateObject, "/update")
            .MapGet(GetAllMemberFromObject, "/get-all-member/{id}")
            .MapDelete(DeleteObject, "/delete/{id}")
            .MapDelete(DeleteObjectMember, "/delete-member/{id}")
            .MapPost(GetAllObjectFromUnit, "/get-all-object-from-unit")
            .MapGet(GetObjectById, "/object-detail/{id}")
            .MapPost(ObjectHistorys, "/get-obj-history");
    }

    public async Task<IResult> CreateObject(ISender sender, [FromBody] CreateObjectCommand command)
    {
        var res = await sender.Send(command);
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }

    public async Task<IResult> AddMemberToObject(ISender sender, [FromBody] AddMemberToObjectCommand command)
    {
        var res = await sender.Send(command);
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }
    public async Task<IResult> UpdateObject(ISender sender, [FromBody] UpdateObjectCommand command)
    {
        var res = await sender.Send(command);
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }
    public async Task<IResult> GetAllMemberFromObject(ISender sender, [FromRoute] Guid id)
    {
        var res = await sender.Send(new GetMemberFromObjectQuery { ObjectTBId = id });
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }
    public async Task<IResult> DeleteObject(ISender sender, [FromRoute] Guid id)
    {
        var res = await sender.Send(new DeleteObjectCommand { ObjectTBId = id });
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }
    public async Task<IResult> DeleteObjectMember(ISender sender, [FromRoute] Guid id)
    {
        var res = await sender.Send(new DeleteMemberFromObjectCommand { MemberId = id });
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }
    public async Task<IResult> GetAllObjectFromUnit(ISender sender, [FromBody] GetAllObjectFromUnitQuery query)
    {
        var res = await sender.Send(query);
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }
    public async Task<IResult> GetObjectById(ISender sender, [FromRoute] Guid id)
    {
        var res = await sender.Send(new GetObjectByIdQuery { ObjectId = id });
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }
    public async Task<IResult> ObjectHistorys(ISender sender, [FromBody] GetObjectHistoryRequest command)
    {
        var history = await sender.Send(command);
        return Results.Ok(new
        {
            message = "success",
            data = history
        });
    }
}