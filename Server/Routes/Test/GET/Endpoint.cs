using FastEndpoints;

namespace Server.Routes.Test.GET;

public class Endpoint : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get("/test");
        AllowAnonymous();
    }

    public override async Task<string> ExecuteAsync(CancellationToken ct)
    {
        return "Hello, World!";
    }
}