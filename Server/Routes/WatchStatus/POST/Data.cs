using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Routes.WatchStatus.POST;

public class PostWatchStatusData(AppDbContext ctx)
{
    public async Task<WatchStatusResponse> PostWatchStatusAsync(int animeId, Guid currentUserId, PostWatchStatusRequest request, CancellationToken ct)
    {
        IDbContextTransaction? transaction = await ctx.Database.BeginTransactionAsync(ct);

        AnimeEntity anime = await ctx.Animes.FirstOrDefaultAsync(a => a.Id == animeId, ct) ??
            throw new NotFoundException("The anime doesn't exist.");

        if (anime.TotalEpisodes is not null && request.EpisodesWatched > anime.TotalEpisodes)
            throw new BadRequestException("You can't watch more episodes than the anime has total episodes.");
        
        ctx.Remove(anime); // Removes the anime from the database to avoid duplicate entries, as it will be re-added with the new watch status.
        
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

        await transaction.CommitAsync(ct);

        return new()
        {
            Message = "The anime has been added to your list."  
        };
    }
}