using System.Text.Json;
using System.Text.Json.Serialization;
using Server.Data;
using Server.Entities;
using Server.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Server.Routes.Tenrai.Inspect;

/// <summary>
/// Data class for fetching anime details from Tenrai API and caching them in the database.
/// </summary>
public class TenraiInspectData(AppDbContext ctx)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };
    private HttpClient _httpClient { get; } = new();

    /// <summary>
    /// Fetches anime details from the database if cached and not stale, otherwise fetches from Tenrai API and caches it.
    /// </summary>
    /// <returns>The anime details.</returns>
    /// <exception cref="TenraiApiException">Thrown when there is an error fetching or processing anime data from Tenrai API.</exception>
    public async Task<Anime> GetAnimeAsync(int id, Guid currentUserId, CancellationToken ct)
    {
        Anime? existingAnime = await TryGetAnimeAsync(id, ct);
        if (existingAnime is not null && existingAnime.LastSynced <= DateTime.UtcNow.AddDays(-7))
            return existingAnime;

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync($"https://api.tenrai.org/v1/anime/{id}", ct);
        }
        catch (HttpRequestException ex)
        {
            throw new TenraiApiException($"Tenrai request failed: {ex.Message}");
        }

        string responseContent = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            TenraiErrorResponse? errorResponse = null;
            try
            {
                errorResponse = JsonSerializer.Deserialize<TenraiErrorResponse>(responseContent, JsonOptions);
            }
            catch (JsonException)
            {
                // Ignore JSON deserialization errors
            }

            string message = errorResponse?.Message ??
                $"Tenrai returned HTTP {(int)response.StatusCode} ({response.StatusCode}).";
            throw new TenraiApiException(message);
        }

        TenraiAnimeDetailResponse payload;

        try
        {
            payload = JsonSerializer.Deserialize<TenraiAnimeDetailResponse>(responseContent, JsonOptions) ??
                throw new TenraiApiException("Tenrai returned an empty response.");
        }
        catch (JsonException ex)
        {
            throw new TenraiApiException($"Failed to parse Tenrai response: {ex.Message}");
        }

        try
        {
            Anime anime = payload.Data.ToAnime();

            Anime? inspectedAnime = await AddOrUpdateAnimeAsync(anime, ct) ?? anime;

            List<ReviewEntity> reviews = await ctx.Reviews
            .AsNoTracking()
            .Where(r => r.AnimeId == inspectedAnime.MalId)
            .Include(r => r.User)
            .Include(r => r.HelpfulByUsers)
            .ToListAsync(ct);

            double? averageScore = reviews.Count > 0 ? reviews.Average(r => r.Score) : null;
            inspectedAnime.Score = averageScore;
            
            ReviewEntity? currentUserReview = reviews.FirstOrDefault(r => r.UserId == currentUserId);
            inspectedAnime.TotalReviews = reviews.Count;
            inspectedAnime.TopReviews = reviews
                .OrderByDescending(r => r.HelpfulByUsers.Count)
                .ThenByDescending(r => r.CreatedAt)
                .Take(3)
                .Select(r => new Review.Review
                {
                    Id = r.Id,
                    UserId = r.User.Id,
                    Text = r.Text,
                    Score = r.Score,
                    Username = r.User.Username,
                    ProfileImageURL = r.User.ProfileImageURL,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    HelpfulCount = r.HelpfulByUsers.Count,
                    IsHelpfulByCurrentUser = r.HelpfulByUsers.Any(h => h.UserId == currentUserId)
                })
                .ToList();
            
            inspectedAnime.CurrentUserReview = currentUserReview is not null ? new Review.Review
            {
                Id = currentUserReview.Id,
                UserId = currentUserReview.User.Id,
                Text = currentUserReview.Text,
                Score = currentUserReview.Score,
                Username = currentUserReview.User.Username,
                ProfileImageURL = currentUserReview.User.ProfileImageURL,
                CreatedAt = currentUserReview.CreatedAt,
                UpdatedAt = currentUserReview.UpdatedAt,
                HelpfulCount = currentUserReview.HelpfulByUsers.Count,
                IsHelpfulByCurrentUser = currentUserReview.HelpfulByUsers.Any(h => h.UserId == currentUserId)
            } : null;

            inspectedAnime.CanCurrentUserMakeReview = currentUserId != Guid.Empty && currentUserReview == null;

            return inspectedAnime;
        }
        catch (DbUpdateException ex)
        {
            throw new TenraiApiException($"Failed to save anime data to the database: {ex.Message}");
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
            existingAnime.MetaDataJSON = string.IsNullOrWhiteSpace(anime.MetaData?.ToString()) ? "" : JsonSerializer.Serialize(anime.MetaData, JsonOptions);
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
                MetaDataJSON = string.IsNullOrWhiteSpace(anime.MetaData?.ToString()) ? "" : JsonSerializer.Serialize(anime.MetaData, JsonOptions),
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

    private record TenraiErrorResponse
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
