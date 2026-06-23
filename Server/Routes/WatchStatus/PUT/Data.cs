using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Routes.WatchStatus.PUT;

public class PutWatchStatusData(AppDbContext ctx)
{
    public async Task<WatchStatusResponse> UpdateWatchStatusAsync(int animeId, Guid currentUserId, PostWatchStatusRequest request, CancellationToken ct)
    {
        WatchStatusEntity status = await ctx.WatchStatuses.Include(w => w.Anime).FirstOrDefaultAsync(w => w.AnimeId == animeId && w.UserId == currentUserId, ct) ??
            throw new NotFoundException("The anime doesn't exist in your list.");
        
        
        if (status.Anime.TotalEpisodes is not null && request.EpisodesWatched > status.Anime.TotalEpisodes)
            throw new BadRequestException("You can't watch more episodes than the anime has total episodes.");

        status.EpisodesWatched = request.EpisodesWatched;
        status.Status = request.Status;
        status.UpdatedAt = DateTime.UtcNow;

        ctx.WatchStatuses.Update(status);

        await ctx.SaveChangesAsync(ct);

        return new()
        {
            Message = "You've successfully updated the watch status of an anime."
        };
    }
}