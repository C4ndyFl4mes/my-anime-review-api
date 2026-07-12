namespace Server.Routes.Tenrai;

/// <summary>
/// Mapping class for converting Tenrai API data models to internal Anime models used in the application.
/// </summary>
public static class TenraiMapping
{
    public static Anime ToAnime(this TenraiAnimeData d) => new()
    {
        MalId = d.MalId,
        Title = d.Titles?.FirstOrDefault(t => t.Type == "English")?.Title ?? d.Titles?.FirstOrDefault(t => t.Type == "Default")?.Title ?? d.Titles?.FirstOrDefault()?.Title ?? string.Empty,
        Synopsis = d.Synopsis,
        ImageUrl = d.Images?.Jpg?.LargeImageUrl ?? d.Images?.Jpg?.ImageUrl,
        TrailerUrl = d.Trailer?.EmbedUrl,
        MalUrl = d.Url,
        TotalEpisodes = d.Episodes,
        Duration = d.Duration,
        AgeRating = d.Rating,
        AiringStatus = d.Status,
        Type = d.Type,
        Season = d.Season,
        Year = d.Year,
        Source = d.Source,
        MetaData = new AnimeMetaData
        {
            Aired = d.Aired?.String,
            Genres = d.Genres,
            Studios = d.Studios,
            Producers = d.Producers,
            Licensors = d.Licensors,
            Themes = d.Themes,
            Demographics = d.Demographics
        }
    };

    public static AnimeSearchItem ToAnimeSearchItem(this TenraiAnimeData d) => new()
    {
        MalId = d.MalId,
        Title = d.Titles?.FirstOrDefault(t => t.Type == "English")?.Title ?? d.Titles?.FirstOrDefault(t => t.Type == "Default")?.Title ?? d.Titles?.FirstOrDefault()?.Title ?? string.Empty,
        ImageUrl = d.Images?.Jpg?.ImageUrl ?? d.Images?.Jpg?.LargeImageUrl,
        AgeRating = d.Rating,
        Type = d.Type,
        CurrentUserWatchStatus = d.CurrentUserWatchStatus,
        Score = d.Score,
        Genres = d.Genres ?? []
    };
}
