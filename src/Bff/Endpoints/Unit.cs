using Microsoft.AspNetCore.Mvc;
using PerfSvc.Application.Unit.Commands;
using PerfSvc.Application.Unit.Queries;

namespace Bff.Endpoints
{
    public class Unit : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this)
                .RequireAuthorization()
                .MapPost(CreateUnit, "/create")
                .MapPatch(UpdateUnit, "/update")
                .MapDelete(DeleteUnit, "/delete/{id}")
                .MapGet(GetAllUnitFromTenant, "/get-all-unit/{id}")
                .MapGet(GetSubUnitFromUnit, "/get-sub-unit/{parentId}")
                .MapGet(DetailUnit, "/detail/{id}");
        }

        public async Task<IResult> CreateUnit(ISender sender, [FromBody] CreateUnitCommand command)
        {
            var res = await sender.Send(command);
            return Results.Ok(new
            {
                status = res.Status,
                message = res.Message,
                data = res.Data
            });
        }

        public async Task<IResult> GetAllUnitFromTenant(ISender sender, [FromRoute] Guid id)
        {
            var res = await sender.Send(new GetUnitFromTenantQuery() { TenantId = id });
            return Results.Ok(new
            {
                status = res.Status,
                message = res.Message,
                data = res.Data
            });
        }

        public async Task<IResult> UpdateUnit(ISender sender, [FromBody] UpdateUnitCommand command)
        {
            var res = await sender.Send(command);
            return Results.Ok(new
            {
                status = res.Status,
                message = res.Message,
                data = res.Data
            });
        }

        public async Task<IResult> DeleteUnit(ISender sender, [FromRoute] Guid id)
        {
            var res = await sender.Send(new DeleteUnitCommand() { Id = id });
            return Results.Ok(new
            {
                status = res.Status,
                message = res.Message,
                data = res.Data
            });
        }

        public async Task<IResult> DetailUnit(ISender sender, [FromRoute] Guid id)
        {
            var res = await sender.Send(new ReadUnitQuery() { Id = id });
            return Results.Ok(new
            {
                status = res.Status,
                message = res.Message,
                data = res.Data
            });
        }

        public async Task<IResult> GetSubUnitFromUnit(ISender sender, [FromRoute] Guid parentId)
        {
            var res = await sender.Send(new GetSubUnitFromUnitQueries() { ParentId = parentId });
            return Results.Ok(new
            {
                status = res.Status,
                message = res.Message,
                data = res.Data
            });
        }
    }
}