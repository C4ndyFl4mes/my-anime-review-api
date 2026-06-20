using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Routes.Follow.POST;

public class FollowPostData(AppDbContext ctx)
{
    public async Task<FollowPostResponse> FollowAsync(Guid currentUserId, FollowPostRequest request, CancellationToken ct)
    {
        UserEntity toFollowUser = await ctx.Users.FirstOrDefaultAsync(u => u.Id == request.ToFollowUserId, ct) ??
            throw new NotFoundException("User to follow doesn't exist.");
        
        if (currentUserId == toFollowUser.Id)
            throw new BadRequestException("You cannot follow yourself.");
        
        if (!request.IsFollowing)
        {
            if (await ctx.FollowInstances.AnyAsync(f => f.FollowerUserId == currentUserId && f.FollowedUserId == request.ToFollowUserId, ct))
                throw new BadRequestException("You're already following this user.");
            
            FollowingEntity followInstance = new()
            {
                FollowerUserId = currentUserId,
                FollowedUserId = toFollowUser.Id,
                CreatedAt = DateTime.UtcNow,
                FollowerUser = null!, // Set by EF Core
                FollowedUser = null! // Set by EF Core
            };

            await ctx.AddAsync(followInstance, ct);

            request.IsFollowing = true;
        }
        else
        {
            FollowingEntity followInstance = await ctx.FollowInstances.FirstOrDefaultAsync(f => f.FollowerUserId == currentUserId && f.FollowedUserId == request.ToFollowUserId, ct) ??
                throw new NotFoundException("The following instance doesn't exist.");
            
            ctx.FollowInstances.Remove(followInstance);

            request.IsFollowing = false;
        }

        await ctx.SaveChangesAsync(ct);

        return new()
        {
            Message = request.IsFollowing ? $"You're now following {toFollowUser.Username}" : $"You're no longer following {toFollowUser.Username}",
            IsFollowing = request.IsFollowing
        };
    }
}