using FastEndpoints;
using Server.Data;
using Server.Utils;

namespace Server.Routes.Review.GetUserReviews;

public class GetUserReviewsEndpoint(AppDbContext ctx) : EndpointWithoutRequest<ReviewExtendedResponse>
{
    public override void Configure()
    {
        Get("/user/{userId}/reviews/");
        AllowAnonymous();
    }

    public override async Task<ReviewExtendedResponse> ExecuteAsync(CancellationToken ct)
    {
        Guid targetUserId = Route<Guid>("userId", isRequired: true);
        Guid currentUserId = UserUtils.GetAuthenticatedUserID();
        int page = Query<int>("page", isRequired: false);
        if (page < 1)
            page = 1;

        GetUserReviewsData data = new(ctx);
        return await data.GetUserReviewsAsync(targetUserId, currentUserId, page, ct);
    }
}