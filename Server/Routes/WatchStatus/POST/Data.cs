using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Routes.WatchStatus.POST;

public class PostWatchStatusData(AppDbContext ctx)
{
    public async Task<WatchStatusResponse> PostWatchStatusAsync(int animeId, Guid currentUserId, PostWatchStatusRequest request, CancellationToken ct)
    {
        AnimeEntity anime = await ctx.Animes.FirstOrDefaultAsync(a => a.Id == animeId, ct) ??
            throw new NotFoundException("The anime doesn't exist.");

        if (anime.TotalEpisodes is not null && request.EpisodesWatched > anime.TotalEpisodes)
            throw new BadRequestException("You can't watch more episodes than the anime has total episodes.");
        
        WatchStatusEntity status = new()
        {
            UserId = currentUserId,
            AnimeId = animeId,
            EpisodesWatched = request.EpisodesWatched,
            Status = request.Status,
            UpdatedAt = DateTime.UtcNow,
            User = null!, // Set by EF.
            Anime = null! // Set by EF.
        };
        

        await ctx.WatchStatuses.AddAsync(status, ct);

        await ctx.SaveChangesAsync(ct);

        return new()
        {
            Message = "You've successfully added the anime to your list."  
        };
    }
}