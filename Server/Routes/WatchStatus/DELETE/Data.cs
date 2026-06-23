using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Routes.WatchStatus.DELETE;

public class DeleteWatchStatusData(AppDbContext ctx)
{
    public async Task<WatchStatusResponse> RemoveWatchStatusInstanceAsync(int animeId, Guid currentUserId, CancellationToken ct)
    {
        WatchStatusEntity status =  await ctx.WatchStatuses.FirstOrDefaultAsync(w => w.AnimeId == animeId && w.UserId == currentUserId, ct) ??
            throw new NotFoundException("The anime doesn't exist in your list.");
        
        ctx.WatchStatuses.Remove(status);

        await ctx.SaveChangesAsync(ct);

        return new()
        {
            Message = "You've successfully deleted the anime from your list."  
        };
    }
}