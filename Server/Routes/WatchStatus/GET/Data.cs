using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Exceptions;
using Server.Routes.Jikan;

namespace Server.Routes.WatchStatus.GET;

public class GetWatchStatusAnimeData(AppDbContext ctx)
{
    public async Task<GetWatchStatusAnimeResponse> GetUserAnimeListAsync(Guid targetUserId, Enums.WatchStatus? status, CancellationToken ct)
    {
        if (!await ctx.Users.AnyAsync(u => u.Id == targetUserId, ct))
            throw new NotFoundException("The user doesn't exist.");

        List<AnimeSearchItemExtended> animeItems = await ctx.WatchStatuses
            .AsNoTracking()
            .Include(w => w.Anime)
            .Where(w => w.UserId == targetUserId && (status == null || w.Status == status))
            .Select(w => new AnimeSearchItemExtended
            {
                Item = new()
                {
                    MalId = w.AnimeId,
                    Title = w.Anime.Title,
                    ImageUrl = w.Anime.ImageUrl,
                    AgeRating = w.Anime.AgeRating,
                    Type = w.Anime.Type,
                    Genres = DeserializeAnimeMetaData(w.Anime.MetaDataJSON!).Genres ?? new List<MalObject>()
                },
                EpisodesWatched = w.EpisodesWatched,
                Status = w.Status
            })
            .ToListAsync(ct);

        return new()
        {
            AnimeItems = animeItems
        };
    }

    private static AnimeMetaData DeserializeAnimeMetaData(string AnimeMetaDataJson)
    {
        return System.Text.Json.JsonSerializer.Deserialize<AnimeMetaData>(AnimeMetaDataJson) ?? new AnimeMetaData();
    }
}