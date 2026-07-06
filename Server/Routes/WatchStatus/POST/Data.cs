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
        string addedMessage = "The anime has been added to your list.";
        string updatedMessage = "The anime has been updated in your list.";
        AnimeEntity anime = await ctx.Animes.FirstOrDefaultAsync(a => a.Id == animeId, ct) ??
            throw new NotFoundException("The anime doesn't exist.");

        if (anime.TotalEpisodes is not null && request.EpisodesWatched > anime.TotalEpisodes)
            throw new BadRequestException("You can't watch more episodes than the anime has total episodes.");

        WatchStatusEntity? existingStatus = await ctx.WatchStatuses.AsNoTracking().FirstOrDefaultAsync(w => w.UserId == currentUserId && w.AnimeId == animeId, ct);
        
        if (existingStatus is not null)
        {
            ctx.WatchStatuses.Remove(existingStatus); // Remove any existing watch status for the user and anime combination to avoid duplicates.
        }
        
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
            Message = existingStatus is null ? addedMessage : updatedMessage
        };
    }
}