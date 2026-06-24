using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Routes.Profile.PUT;

public class ChangeProfileImageData(AppDbContext ctx)
{
    public async Task<ChangeProfileImageResponse> ChangeProfileImageAsync(Guid currentUserId, ChangeProfileImageRequest request, CancellationToken ct)
    {
        UserEntity user = await ctx.Users.FirstOrDefaultAsync(u => u.Id == currentUserId, ct) ??
            throw new NotFoundException("The user doesn't exist.");

        user.ProfileImageURL = request.ProfileImageURL;

        if (ctx.ChangeTracker.HasChanges())
        {
            await ctx.SaveChangesAsync(ct);
            return new()
            {
                ProfileImageURL = user.ProfileImageURL,
                Message = "The profile image has been updated."
            };
        }

        return new()
        {
            ProfileImageURL = request.ProfileImageURL,
            Message = "The profile image remains the same."
        };
    }
}