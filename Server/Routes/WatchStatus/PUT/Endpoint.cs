using FastEndpoints;
using Server.Data;
using Server.Utils;

namespace Server.Routes.WatchStatus.PUT;

public class PutWatchStatusEndpoint(AppDbContext ctx) : Endpoint<PostWatchStatusRequest, WatchStatusResponse>
{
    public override void Configure()
    {
        Put("/watch-status/{malId}");
        Roles("User", "Admin");
    }

    public override async Task<WatchStatusResponse> ExecuteAsync(PostWatchStatusRequest request, CancellationToken ct)
    {
        int animeId = Route<int>("malId", isRequired: true);
        Guid currentUserId = UserUtils.GetAuthenticatedUserID();

        PutWatchStatusData data = new(ctx);
        return await data.UpdateWatchStatusAsync(animeId, currentUserId, request, ct);
    }
}