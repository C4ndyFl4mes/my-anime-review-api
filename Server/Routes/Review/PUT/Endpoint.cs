using FastEndpoints;
using Server.Data;
using Server.Utils;

namespace Server.Routes.Review.PUT;

public class UpdateReviewEndpoint(AppDbContext ctx) : Endpoint<ReviewPostRequest, ReviewPostResponse>
{
    public override void Configure()
    {
        Put("/review/edit/{reviewId}");
        Roles("User", "Admin");
    }

    public override async Task<ReviewPostResponse> ExecuteAsync(ReviewPostRequest request, CancellationToken ct)
    {
        Guid reviewId = Route<Guid>("reviewId");
        Guid currentUserId = UserUtils.GetAuthenticatedUserID();

        UpdateReviewData data = new(ctx);
        return await data.UpdateReviewAsync(reviewId, currentUserId, request, ct);
    }
}