using FastEndpoints;
using Server.Data;
using Server.Utils;

namespace Server.Routes.Review.GET;

public class GetAnimeReviewsEndpoint(AppDbContext ctx) : EndpointWithoutRequest<ReviewResponse>
{
    public override void Configure()
    {
        Get("/anime/reviews/{malId}");
        AllowAnonymous();
    }

    public override async Task<ReviewResponse> ExecuteAsync(CancellationToken ct)
    {
        int malId = Route<int>("malId");
        int page = Query<int>("page", isRequired: false);
        if (page < 1)
            page = 1;
        
        Guid currentUserId = UserUtils.GetAuthenticatedUserID();
        
        GetAnimeReviewsData data = new(ctx);
        return await data.GetReviewsAsync(malId, currentUserId, page, ct);
    }
}