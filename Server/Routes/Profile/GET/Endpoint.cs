using FastEndpoints;
using Server.Data;
using Server.Utils;

namespace Server.Routes.Profile.GET;

public class GetProfileEndpoint(AppDbContext ctx) : EndpointWithoutRequest<GetProfileResponse>
{
    public override void Configure()
    {
        Get("/user/{userId}/profile");
        AllowAnonymous();
    }

    public override async Task<GetProfileResponse> ExecuteAsync(CancellationToken ct)
    {
        Guid targetUserId = Route<Guid>("userId", isRequired: true);
        Guid currentuserId = UserUtils.GetAuthenticatedUserID();

        GetProfileData data = new(ctx);
        return await data.GetProfileAsync(targetUserId, currentuserId, ct);
    }
}