using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;

namespace Server.Routes.WatchStatus.GetAnimeWatchStatus;

public class GetAnimeWatchStatusData(AppDbContext ctx)
{
    public async Task<GetAnimeWatchStatusResponse> GetAnimeWatchStatusAsync(int malId, Guid currentUserId, CancellationToken ct)
    {
        AnimeEntity? anime = await ctx.Animes.FirstOrDefaultAsync(a => a.Id == malId, ct);
        WatchStatusEntity? status = await ctx.WatchStatuses.FirstOrDefaultAsync(w => w.UserId == currentUserId && w.AnimeId == malId, ct);

        if (status is not null)
        {
            return new()
            {
                EpisodesWatched = status.EpisodesWatched,
                Status = status.Status.ToString(),
                MaxEpisodes = anime?.TotalEpisodes ?? 99999, // If the anime's total episodes are unknown. This is to ensure that the user can still add episodes watched to the anime.
                Title = anime?.Title ?? "N/A",
                IsInWatchList = true
            };
        } else
        {
            return new()
            {
                EpisodesWatched = 0,
                Status = Enums.WatchStatus.Planned.ToString(),
                MaxEpisodes = anime?.TotalEpisodes ?? 99999, // If the anime's total episodes are unknown. This is to ensure that the user can still add episodes watched to the anime.
                Title = anime?.Title ?? "N/A",
                IsInWatchList = false
            };
        }
    }
}