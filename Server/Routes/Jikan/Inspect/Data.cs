using System.Text.Json;
using System.Text.Json.Serialization;
using Server.Data;
using Server.Entities;
using Server.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Server.Routes.Jikan.Inspect;

/// <summary>
/// Data class for fetching anime details from Jikan API and caching them in the database.
/// </summary>
public class JikanInspectData(AppDbContext ctx)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };
    private HttpClient _httpClient { get; } = new();

    /// <summary>
    /// Fetches anime details from the database if cached and not stale, otherwise fetches from Jikan API and caches it.
    /// </summary>
    /// <returns>The anime details.</returns>
    /// <exception cref="JikanApiException">Thrown when there is an error fetching or processing anime data from Jikan API.</exception>
    public async Task<Anime> GetAnimeAsync(int id, CancellationToken ct)
    {
        Anime? existingAnime = await TryGetAnimeAsync(id, ct);
        if (existingAnime is not null && existingAnime.LastSynced <= DateTime.UtcNow.AddDays(-7))
            return existingAnime;

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync($"https://api.jikan.moe/v4/anime/{id}", ct);
        }
        catch (HttpRequestException ex)
        {
            throw new JikanApiException($"Jikan request failed: {ex.Message}");
        }

        string responseContent = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            JikanErrorResponse? errorResponse = null;
            try
            {
                errorResponse = JsonSerializer.Deserialize<JikanErrorResponse>(responseContent, JsonOptions);
            }
            catch (JsonException)
            {
                // Ignore JSON deserialization errors
            }

            string message = errorResponse?.Message ??
                $"Jikan returned HTTP {(int)response.StatusCode} ({response.StatusCode}).";
            throw new JikanApiException(message);
        }

        JikanAnimeDetailResponse payload;

        try
        {
            payload = JsonSerializer.Deserialize<JikanAnimeDetailResponse>(responseContent, JsonOptions) ??
                throw new JikanApiException("Jikan returned an empty response.");
        }
        catch (JsonException ex)
        {
            throw new JikanApiException($"Failed to parse Jikan response: {ex.Message}");
        }

        try
        {
            Anime anime = payload.Data.ToAnime();

            return await AddOrUpdateAnimeAsync(anime, ct) ?? anime;
        }
        catch (DbUpdateException ex)
        {
            throw new JikanApiException($"Failed to save anime data to the database: {ex.Message}");
        }
    }

    /// <summary>
    /// Adds a new anime to the database or updates it if it already exists.
    /// </summary>
    /// <returns>The added or updated anime.</returns>
    /// <exception cref="DbUpdateException">Thrown when there is an error saving anime data to the database.</exception>
    private async Task<Anime?> AddOrUpdateAnimeAsync(Anime anime, CancellationToken ct)
    {
        AnimeEntity? existingAnime = await ctx.Animes.FirstOrDefaultAsync(a => a.Id == anime.MalId, ct);
        if (existingAnime != null)
        {
            existingAnime.Title = anime.Title;
            existingAnime.Synopsis = anime.Synopsis;
            existingAnime.ImageUrl = anime.ImageUrl;
            existingAnime.TrailerUrl = anime.TrailerUrl;
            existingAnime.MalUrl = anime.MalUrl;
            existingAnime.TotalEpisodes = anime.TotalEpisodes;
            existingAnime.Duration = anime.Duration;
            existingAnime.AgeRating = anime.AgeRating;
            existingAnime.AiringStatus = anime.AiringStatus;
            existingAnime.Type = anime.Type;
            existingAnime.Season = anime.Season;
            existingAnime.Year = anime.Year;
            existingAnime.Source = anime.Source;
            existingAnime.MetaDataJSON = string.IsNullOrWhiteSpace(anime.MetaData?.ToString()) ? null : JsonSerializer.Serialize(anime.MetaData, JsonOptions);
            existingAnime.LastSynced = DateTime.UtcNow;

            ctx.Animes.Update(existingAnime);
        }
        else
        {
            await ctx.Animes.AddAsync(new AnimeEntity
            {
                Id = anime.MalId,
                Title = anime.Title,
                Synopsis = anime.Synopsis,
                ImageUrl = anime.ImageUrl,
                TrailerUrl = anime.TrailerUrl,
                MalUrl = anime.MalUrl,
                TotalEpisodes = anime.TotalEpisodes,
                Duration = anime.Duration,
                AgeRating = anime.AgeRating,
                AiringStatus = anime.AiringStatus,
                Type = anime.Type,
                Season = anime.Season,
                Year = anime.Year,
                Source = anime.Source,
                MetaDataJSON = string.IsNullOrWhiteSpace(anime.MetaData?.ToString()) ? null : JsonSerializer.Serialize(anime.MetaData, JsonOptions),
                LastSynced = DateTime.UtcNow
            }, ct);
        }

        await ctx.SaveChangesAsync(ct);

        return await TryGetAnimeAsync(anime.MalId, ct);
    }

    /// <summary>
    /// Tries to get anime details from the database if it exists and is not stale (older than 7 days), otherwise returns null.
    /// </summary>
    /// <returns>The anime details if found and not stale, otherwise null.</returns>
    private async Task<Anime?> TryGetAnimeAsync(int malId, CancellationToken ct)
    {
        AnimeEntity? anime = await ctx.Animes.FirstOrDefaultAsync(a => a.Id == malId, ct);
        if (anime == null)
            return null;

        if (anime.LastSynced < DateTime.UtcNow.AddDays(-7))
            return null;

        return new Anime
        {
            MalId = anime.Id,
            Title = anime.Title,
            Synopsis = anime.Synopsis,
            ImageUrl = anime.ImageUrl,
            TrailerUrl = anime.TrailerUrl,
            MalUrl = anime.MalUrl,
            TotalEpisodes = anime.TotalEpisodes,
            Duration = anime.Duration,
            AgeRating = anime.AgeRating,
            AiringStatus = anime.AiringStatus,
            Type = anime.Type,
            Season = anime.Season,
            Year = anime.Year,
            Source = anime.Source,
            MetaData = string.IsNullOrWhiteSpace(anime.MetaDataJSON) ? null : JsonSerializer.Deserialize<AnimeMetaData>(anime.MetaDataJSON, JsonOptions),
            LastSynced = anime.LastSynced
        };
    }

    private record JikanErrorResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;
        [JsonPropertyName("report_url")]
        public string ReportUrl { get; set; } = string.Empty;
    }
}