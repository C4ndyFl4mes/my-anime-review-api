using FastEndpoints;
using Server.Data;
using Server.Utils;

namespace Server.Routes.WatchStatus.DELETE;

public class DeleteWatchStatusEndpoint(AppDbContext ctx) : EndpointWithoutRequest<WatchStatusResponse>
{
    public override void Configure()
    {
        Delete("/watch-status/{malId}");
        Roles("User", "Admin");
    }

    public override async Task<WatchStatusResponse> ExecuteAsync(CancellationToken ct)
    {
        int animeId = Route<int>("malId", isRequired: true);
        Guid currentUserId = UserUtils.GetAuthenticatedUserID();

        DeleteWatchStatusData data = new(ctx);
        return await data.RemoveWatchStatusInstanceAsync(animeId, currentUserId, ct);
    }
}