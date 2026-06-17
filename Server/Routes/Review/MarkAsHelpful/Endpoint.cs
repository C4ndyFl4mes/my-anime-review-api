using FastEndpoints;
using Server.Data;
using Server.Utils;

namespace Server.Routes.Review.MarkAsHelpful;

public class MarkAsHelpfulEndpoint(AppDbContext ctx) : Endpoint<MarkAsHelpfulRequest, MarkAsHelpfulResponse>
{
    public override void Configure()
    {
        Put("/review/markashelpful");
        Roles("User", "Admin");
    }

    public override async Task<MarkAsHelpfulResponse> ExecuteAsync(MarkAsHelpfulRequest request, CancellationToken ct)
    {
        Guid currentUserId = UserUtils.GetAuthenticatedUserID();

        MarkAsHelpfulData data = new(ctx);
        return await data.MarkReviewAsHelpfulAsync(request, currentUserId, ct);
    }
}