using Microsoft.AspNetCore.Mvc;
using PerfSvc.Application.UnitMember.Commands;
using PerfSvc.Application.UnitMember.Queries;

namespace Bff.Endpoints;

public class UnitMember : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(GetMemberFromUnit, "/Get-member-from-unit");

        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(AddMemberToUnit, "/add-member-to-unit");
    }
    
    public async Task<IResult> AddMemberToUnit(ISender sender, [FromBody] AddMemberToUnitCommand command)
    {
        var res = await sender.Send(command);
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }
        
    public async Task<IResult> GetMemberFromUnit(ISender sender, [FromBody] GetListUnitMemberQuery query)
    {
        var res = await sender.Send(query);
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res.Data
        });
    }
}