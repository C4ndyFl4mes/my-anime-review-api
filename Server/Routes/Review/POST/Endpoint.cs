using FastEndpoints;
using Server.Data;
using Server.Utils;

namespace Server.Routes.Review.POST;

public class PostReviewEndpoint(AppDbContext ctx) : Endpoint<ReviewPostRequest, ReviewPostResponse>
{
    public override void Configure()
    {
        Post("/review/{malId}");
        Roles("User", "Admin");
    }

    public override async Task<ReviewPostResponse> ExecuteAsync(ReviewPostRequest request, CancellationToken ct)
    {
        int malId = Route<int>("malId");
        Guid currentUserId = UserUtils.GetAuthenticatedUserID();

        PostReviewData data = new(ctx);
        return await data.PostReviewAsync(malId, currentUserId, request, ct);
    }
}