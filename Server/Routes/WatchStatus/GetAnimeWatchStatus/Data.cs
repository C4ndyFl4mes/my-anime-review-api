using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;

namespace Server.Routes.WatchStatus.GetAnimeWatchStatus;

public class GetAnimeWatchStatusData(AppDbContext ctx)
{
    public async Task<GetAnimeWatchStatusResponse> GetAnimeWatchStatusAsync(int malId, Guid currentUserId, CancellationToken ct)
    {
        WatchStatusEntity? status = await ctx.WatchStatuses.FirstOrDefaultAsync(w => w.UserId == currentUserId && w.AnimeId == malId, ct);

        if (status is not null)
        {
            return new()
            {
                EpisodesWatched = status.EpisodesWatched,
                Status = status.Status.ToString()
            };
        } else
        {
            return new()
            {
                EpisodesWatched = 0,
                Status = Enums.WatchStatus.Planned.ToString()
            };
        }
    }
}