using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Routes.Profile.GetCurrentProfileImage;

public class GetCurrentProfileImageData(AppDbContext ctx)
{
   public async Task<ProfileImageResponse> GetCurrentProfileImageAsync(Guid currentUserId, CancellationToken ct)
    {
        UserEntity user = await ctx.Users.FindAsync(new object[] { currentUserId }, ct) ??
            throw new NotFoundException("User not found");

        return new ProfileImageResponse
        {
            ProfileImageURL = user.ProfileImageURL ?? string.Empty
        };
    }
}