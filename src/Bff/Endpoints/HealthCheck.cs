using Bff.Infrastructure;

namespace Bff.Endpoints;

public class HealthCheck : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup("HealthCheck").WithTags("Ping").MapGet("/ping", () =>
        {
            return 1;
        })
        .WithName("Ping")
        .WithOpenApi();
    }
}