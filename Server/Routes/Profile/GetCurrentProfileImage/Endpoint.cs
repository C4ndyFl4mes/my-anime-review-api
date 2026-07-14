using FastEndpoints;
using Server.Data;
using Server.Utils;

namespace Server.Routes.Profile.GetCurrentProfileImage;

public class GetCurrentProfileImageEndpoint(AppDbContext ctx) : EndpointWithoutRequest<ProfileImageResponse>
{
    public override void Configure()
    {
        Get("/user/profile-image/");
        Roles("User", "Admin");
    }

    public override async Task<ProfileImageResponse> ExecuteAsync(CancellationToken ct)
    {
        Guid currentUserId = UserUtils.GetAuthenticatedUserID();

        GetCurrentProfileImageData data = new(ctx);
        return await data.GetCurrentProfileImageAsync(currentUserId, ct);
    }
}