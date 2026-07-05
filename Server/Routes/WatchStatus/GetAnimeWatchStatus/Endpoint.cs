using FastEndpoints;
using Server.Data;
using Server.Utils;

namespace Server.Routes.WatchStatus.GetAnimeWatchStatus;

public class GetAnimeWatchStatusEndpoint(AppDbContext ctx) : EndpointWithoutRequest<GetAnimeWatchStatusResponse>
{
    public override void Configure()
    {
        Get("/watch-status/{malId}");
        Roles("User", "Admin");
    }

    public override async Task<GetAnimeWatchStatusResponse> ExecuteAsync(CancellationToken ct)
    {
        int malId = Route<int>("malId", isRequired: true);
        Guid currentUserId = UserUtils.GetAuthenticatedUserID();

        GetAnimeWatchStatusData data = new(ctx);

        return await data.GetAnimeWatchStatusAsync(malId, currentUserId, ct);
    }
}