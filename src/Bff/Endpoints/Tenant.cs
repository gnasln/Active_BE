using System.Security.Cryptography.X509Certificates;
using Bff.Application.Tenants.Commands;
using Bff.Application.Tenants.Queries;
using Bff.Dtos;
using Bff.Identity;
using Bff.Infrastructure.Data;
using Funq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Bff.Endpoints;

public class Tenant : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateTenant, "/create")
            .MapGet(GetListTenantFromOwner, "/get-all-from-owner")
            .MapPatch(UpdateTenant, "/update")
            .MapGet(GetAllMemberOfTenant, "/{id}/member-list")
            .MapPost(AddMemberToTenant, "/add-member")
            .MapDelete(DeleteMemberInTenant, "/remove-member")
            .MapGet(GetTenantById, "/tenant-detail/{id}")
            .MapDelete(DeleteTenant, "/delete/{id}")
            // workspace cá nhân 
            .MapPost(CreateWorkspacePesonal, "/personal-create")
            .MapGet(getWorkspacePersonal, "/personal")
            ;
        app.MapGroup(this)
            .RequireAuthorization("admin")
            .MapGet(GetListTenantByAdmin, "/get-all-by-admin");

    }

    public async Task<IResult> GetTenantById(ISender sender, [FromRoute] Guid id)
    {
        var res = await sender.Send(new GetTenantByIdQuery() { Id = id });
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res?.Data
        });
    }

    public async Task<IResult> CreateTenant(ISender sender, [FromBody] TenantCreateCommand command)
    {
        var res = await sender.Send(command);
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res?.Data
        });
    }

    public async Task<IResult> UpdateTenant([FromServices] ISender sender, [FromBody] TenantUpdateCommand command)
    {
        var res = await sender.Send(command);
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res?.Data
        });
    }

    public async Task<IResult> GetListTenantFromOwner(ISender sender)
    {
        var res = await sender.Send(new GetListTenantFromOwnerQuery { });
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res?.Data
        });
    }

    public async Task<IResult> GetListTenantByAdmin(ISender sender)
    {
        var res = await sender.Send(new GetListTenantByAdminQuery { });
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res?.Data
        });
    }

    public async Task<IResult> GetAllMemberOfTenant(ISender sender, [FromRoute] Guid id)
    {
        var res = await sender.Send(new GetAllMemberOfTenantQuery { Id = id });
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res?.Data
        });
    }

    public async Task<IResult> AddMemberToTenant(ISender sender, [FromBody] AddMemberToTenantCommand command)
    {
        var res = await sender.Send(command);
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res?.Data
        });
    }

    public async Task<IResult> DeleteMemberInTenant(ISender sender, [FromBody] DeleteMemberCommand command)
    {
        var res = await sender.Send(command);
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res?.Data
        });
    }

    public async Task<IResult> CreateWorkspacePesonal(ISender sender, CreateWorkSpaceCommand command)
    {
        var res = await sender.Send(command);
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res?.Data
        });
    }

    public async Task<IResult> getWorkspacePersonal(ISender sender)
    {
        var query = new GetWorkspacePersonalQuery() { };
        var res = await sender.Send(query);
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message,
            data = res?.Data
        });
    }

    public async Task<IResult> DeleteTenant(ISender sender, [FromRoute] Guid id)
    {
        var res = await sender.Send(new DeleteTenantCommand() { TenantId  = id});
        return Results.Ok(new
        {
            status = res.Status,
            message = res.Message
        });
    }
}