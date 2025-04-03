using Bff.Application.Common.Interfaces;
using Bff.Application.Sample.Queries;
using Bff.Services;

namespace Bff.Endpoints;

public class Sample : EndpointGroupBase
{    
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(SampleApi)
        ;
    }

    public async Task<string> SampleApi(ISender sender, SampleQuery query)
    {
        return await sender.Send(query);
    }
}
