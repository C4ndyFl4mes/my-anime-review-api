using FastEndpoints;
using Server.Data;
using Server.Utils;

namespace Server.Routes.Review.DELETE;

public class DeleteReviewEndpoint(AppDbContext ctx) : EndpointWithoutRequest<ReviewPostResponse>
{
    public override void Configure()
    {
        Delete("/review/delete/{reviewId}");
        Roles("User", "Admin");
    }

    public override async Task<ReviewPostResponse> ExecuteAsync(CancellationToken ct)
    {
        Guid reviewId = Route<Guid>("reviewId");
        Guid currentUserId = UserUtils.GetAuthenticatedUserID();
        string currenUserRole = UserUtils.GetAuthenticatedUserRole();

        DeleteReviewData data = new(ctx);
        return await data.DeleteReviewAsync(reviewId, currentUserId, currenUserRole, ct);
    }
}