using FastEndpoints;
using Server.Data;
using Server.Utils;

namespace Server.Routes.Follow.POST;

public class FollowPostEndpoint(AppDbContext ctx) : Endpoint<FollowPostRequest, FollowPostResponse>
{
    public override void Configure()
    {
        Post("/follow");
        Roles("User", "Roles");
    }

    public override async Task<FollowPostResponse> ExecuteAsync(FollowPostRequest request, CancellationToken ct)
    {
        Guid currentUserId = UserUtils.GetAuthenticatedUserID();

        FollowPostData data = new(ctx);
        return await data.FollowAsync(currentUserId, request, ct);
    }
}