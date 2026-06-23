using FastEndpoints;
using Server.Data;
using Server.Utils;

namespace Server.Routes.Profile.PUT;

public class ChangeProfileImageEndpoint(AppDbContext ctx) : Endpoint<ChangeProfileImageRequest, ChangeProfileImageResponse>
{
    public override void Configure()
    {
        Put("/user/profile-image/");
        Roles("User", "Admin");
    }

    public override async Task<ChangeProfileImageResponse> ExecuteAsync(ChangeProfileImageRequest request, CancellationToken ct)
    {
        Guid currentUserId = UserUtils.GetAuthenticatedUserID();

        ChangeProfileImageData data = new(ctx);
        return await data.ChangeProfileImageAsync(currentUserId, request, ct);
    }
}