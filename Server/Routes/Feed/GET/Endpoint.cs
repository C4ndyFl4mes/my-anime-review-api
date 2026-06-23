using FastEndpoints;
using Server.Data;
using Server.Utils;

namespace Server.Routes.Feed.GET;

public class GetFeedEndpoint(AppDbContext ctx) : EndpointWithoutRequest<FeedResponse>
{
    public override void Configure()
    {
        Get("/feed");
        Roles("User", "Admin");
    }

    public override async Task<FeedResponse> ExecuteAsync(CancellationToken ct)
    {
        int pageSize = Query<int>("pageSize", isRequired: false);
        if (pageSize < 1)
            pageSize = 20;
        int page = Query<int>("page", isRequired: false);
        if (page < 1)
            page = 1;
        
        Guid currentUserId = UserUtils.GetAuthenticatedUserID();

        GetFeedData data = new(ctx);
        return await data.GetFeedAsync(currentUserId, page, pageSize, ct);
    }
}