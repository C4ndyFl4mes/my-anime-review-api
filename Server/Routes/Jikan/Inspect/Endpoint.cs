using FastEndpoints;
using Server.Data;
using Server.Utils;

namespace Server.Routes.Jikan.Inspect;

/// <summary>
/// Endpoint for inspecting anime details using Jikan API.
/// </summary>
public class JikanInspectEndpoint(AppDbContext ctx) : EndpointWithoutRequest<Anime>
{
    public override void Configure()
    {
        Get("/anime/inspect/{malId}");
        AllowAnonymous();
    }

    public override async Task<Anime> ExecuteAsync(CancellationToken ct)
    {
        int malId = Route<int>("malId");
        Guid currentUserId = UserUtils.GetAuthenticatedUserID();

        JikanInspectData data = new(ctx);

        return await data.GetAnimeAsync(malId, currentUserId, ct);
    }
}