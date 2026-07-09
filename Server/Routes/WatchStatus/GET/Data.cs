using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Exceptions;
using Server.Routes.Jikan;

namespace Server.Routes.WatchStatus.GET;

public class GetWatchStatusAnimeData(AppDbContext ctx)
{
    public async Task<GetWatchStatusAnimeResponse> GetUserAnimeListAsync(Guid targetUserId, string status, CancellationToken ct)
    {
        if (!await ctx.Users.AnyAsync(u => u.Id == targetUserId, ct))
            throw new NotFoundException("The user doesn't exist.");

        IQueryable<Entities.WatchStatusEntity> query = ctx.WatchStatuses
            .AsNoTracking()
            .Include(w => w.Anime)
            .Where(w => w.UserId == targetUserId);

        if (!status.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            Enums.WatchStatus parsedStatus = Enum.Parse<Enums.WatchStatus>(status, true);
            query = query.Where(w => w.Status == parsedStatus);
        }

        List<AnimeSearchItemExtended> animeItems = await query
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
                Status = w.Status.ToString()
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