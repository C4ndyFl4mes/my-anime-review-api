using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Exceptions;
using Server.Routes.WatchStatus;

namespace Server.Routes.Jikan.Search;

/// <summary>
/// Data class for searching anime using Jikan API. Supports optional query parameter 'q' for search term and 'page' for pagination.
/// </summary>
public class JikanSearchData(AppDbContext ctx)
{
    private const int MaxPagesToScanForDeduplication = 20;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };
    private HttpClient _httpClient { get; } = new();

    /// <summary>
    /// Searches for anime using Jikan API with optional query and pagination. If query is null or empty, returns paginated list of all anime.
    /// </summary>
    /// <returns>The search results as a <see cref="JikanSearchResponse"/>.</returns>
    /// <exception cref="JikanApiException">Thrown when the Jikan API request fails or returns an error.</exception>
    public async Task<JikanSearchResponse> SearchAnimeAsync(string? query, int page, Guid currentUserId, CancellationToken ct)
    {
        try
        {
            page = page <= 0 ? 1 : page;

            JikanAnimeResponse currentPayload = await FetchAnimePageAsync(query, 1, ct);
            int perPage = currentPayload.Pagination.Items.PerPage > 0
                ? currentPayload.Pagination.Items.PerPage
                : 25;

            int requestedUniqueCount = page * perPage;
            HashSet<int> seenMalIds = [];
            List<JikanAnimeData> uniqueAnime = [];
            int scannedPages = 0;

            while (true)
            {
                scannedPages++;

                foreach (JikanAnimeData anime in currentPayload.Data)
                {
                    if (seenMalIds.Add(anime.MalId))
                    {
                        uniqueAnime.Add(anime);
                    }
                }

                bool reachedRequestedPage = uniqueAnime.Count >= requestedUniqueCount;
                bool reachedScanLimit = scannedPages >= MaxPagesToScanForDeduplication;
                bool hasMoreSourcePages = currentPayload.Pagination.HasNextPage;

                if (reachedRequestedPage || reachedScanLimit || !hasMoreSourcePages)
                {
                    break;
                }

                currentPayload = await FetchAnimePageAsync(query, scannedPages + 1, ct);
            }

            int skip = (page - 1) * perPage;
            List<JikanAnimeData> requestedPageData = uniqueAnime
                .Skip(skip)
                .Take(perPage)
                .ToList();

            HashSet<int> pageAnimeIds = requestedPageData.Select(a => a.MalId).Distinct().ToHashSet();

            Dictionary<int, string> watchStatusByAnimeId = [];
            if (currentUserId != Guid.Empty && pageAnimeIds.Count > 0)
            {
                watchStatusByAnimeId = await ctx.WatchStatuses
                    .AsNoTracking()
                    .Where(w => w.UserId == currentUserId && pageAnimeIds.Contains(w.AnimeId))
                    .Select(w => new { w.AnimeId, Status = w.Status.ToString() })
                    .ToDictionaryAsync(w => w.AnimeId, w => w.Status, ct);
            }

            Dictionary<int, double> averageScoreByAnimeId = [];
            if (pageAnimeIds.Count > 0)
            {
                averageScoreByAnimeId = await ctx.Reviews
                    .AsNoTracking()
                    .Where(r => pageAnimeIds.Contains(r.AnimeId))
                    .GroupBy(r => r.AnimeId)
                    .Select(g => new
                    {
                        AnimeId = g.Key,
                        AverageScore = Math.Round(g.Average(r => r.Score), 2, MidpointRounding.AwayFromZero)
                    })
                    .ToDictionaryAsync(x => x.AnimeId, x => x.AverageScore, ct);
            }
            
            requestedPageData.ForEach(a =>
            {
                a.CurrentUserWatchStatus = watchStatusByAnimeId.GetValueOrDefault(a.MalId);
                a.Score = averageScoreByAnimeId.TryGetValue(a.MalId, out double averageScore)
                    ? averageScore
                    : null;
            });

            bool hasNextPage = uniqueAnime.Count > skip + requestedPageData.Count || currentPayload.Pagination.HasNextPage;

            return new JikanSearchResponse
            {
                Pagination = new Pagination
                {
                    CurrentPage = page,
                    HasNextPage = hasNextPage,
                    LastVisiblePage = hasNextPage ? page + 1 : page,
                    Items = new Items
                    {
                        Count = requestedPageData.Count,
                        PerPage = perPage,
                        Total = currentPayload.Pagination.Items.Total
                    }
                },
                Data = requestedPageData.Select(a => a.ToAnimeSearchItem()).ToList()
            };
        }
        catch (JsonException ex)
        {
            throw new JikanApiException($"Failed to parse Jikan response: {ex.Message}");
        }
    }

    private async Task<JikanAnimeResponse> FetchAnimePageAsync(string? query, int page, CancellationToken ct)
    {
        HttpResponseMessage response;
        try
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
                response = await _httpClient.GetAsync($"https://api.jikan.moe/v4/anime?q={Uri.EscapeDataString(query)}&page={page}", ct);
            }
            else
            {
                response = await _httpClient.GetAsync($"https://api.jikan.moe/v4/anime?page={page}", ct);
            }
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

        return JsonSerializer.Deserialize<JikanAnimeResponse>(responseContent, JsonOptions) ??
            throw new JikanApiException("Jikan returned an empty response.");
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