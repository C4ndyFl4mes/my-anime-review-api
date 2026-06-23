using FastEndpoints;
using Server.Data;
using Server.Utils;

namespace Server.Routes.WatchStatus.POST;

public class PostWatchStatusEndpoint(AppDbContext ctx) : Endpoint<PostWatchStatusRequest, WatchStatusResponse>
{
    public override void Configure()
    {
        Post("/watch-status/{malId}");
        Roles("User", "Admin");
    }

    public override async Task<WatchStatusResponse> ExecuteAsync(PostWatchStatusRequest request, CancellationToken ct)
    {
        int animeId = Route<int>("malId", isRequired: true);
        Guid currentUserId = UserUtils.GetAuthenticatedUserID();

        PostWatchStatusData data = new(ctx);
        return await data.PostWatchStatusAsync(animeId, currentUserId, request, ct);
    }
}