using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using Server.Exceptions;
using Server.Routes.WatchStatus;

namespace Server.Routes.Tenrai.Search;

/// <summary>
/// Data class for searching anime using Tenrai API. Supports optional query parameter 'q' for search term and 'page' for pagination.
/// </summary>
public class TenraiSearchData(AppDbContext ctx, IMemoryCache cache)
{
    private const int MaxPagesToScanForDeduplication = 20;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };
    private HttpClient _httpClient { get; } = new();

    /// <summary>
    /// Searches for anime using Tenrai API with optional query and pagination. If query is null or empty, returns paginated list of all anime.
    /// </summary>
    /// <returns>The search results as a <see cref="TenraiSearchResponse"/>.</returns>
    /// <exception cref="TenraiApiException">Thrown when the Tenrai API request fails or returns an error.</exception>
    public async Task<TenraiSearchResponse> SearchAnimeAsync(string? query, int page, Guid currentUserId, CancellationToken ct)
    {
        try
        {
            page = page <= 0 ? 1 : page;

            TenraiAnimeResponse currentPayload = await FetchAnimePageAsync(query, 1, ct);
            int perPage = currentPayload.Pagination.Items.PerPage > 0
                ? currentPayload.Pagination.Items.PerPage
                : 25;

            int requestedUniqueCount = page * perPage;
            HashSet<int> seenMalIds = [];
            List<TenraiAnimeData> uniqueAnime = [];
            int scannedPages = 0;

            while (true)
            {
                scannedPages++;

                foreach (TenraiAnimeData anime in currentPayload.Data)
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
            List<TenraiAnimeData> requestedPageData = uniqueAnime
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

            return new TenraiSearchResponse
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
            throw new TenraiApiException($"Failed to parse Tenrai response: {ex.Message}");
        }
    }

    private async Task<TenraiAnimeResponse> FetchAnimePageAsync(string? query, int page, CancellationToken ct)
    {
        string cacheKey = $"tenrai:anime:{NormalizeQuery(query)}:page:{page}";

        if (cache.TryGetValue(cacheKey, out string? cachedJson) && !string.IsNullOrWhiteSpace(cachedJson))
        {
            return JsonSerializer.Deserialize<TenraiAnimeResponse>(cachedJson, JsonOptions) ??
                throw new TenraiApiException("Cached Tenrai response was empty.");
        }


        HttpResponseMessage response;
        try
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
                response = await _httpClient.GetAsync($"https://api.tenrai.org/v1/anime?q={Uri.EscapeDataString(query)}&page={page}", ct);
            }
            else
            {
                response = await _httpClient.GetAsync($"https://api.tenrai.org/v1/anime?page={page}", ct);
            }
        }
        catch (HttpRequestException ex)
        {
            throw new TenraiApiException($"Tenrai request failed: {ex.Message}");
        }

        string responseContent = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            TenraiErrorResponse? errorResponse = null;
            try { errorResponse = JsonSerializer.Deserialize<TenraiErrorResponse>(responseContent, JsonOptions); }
            catch (JsonException) { }

            string message = errorResponse?.Message ?? $"Tenrai returned HTTP {(int)response.StatusCode} ({response.StatusCode}).";
            throw new TenraiApiException(message);
        }

        cache.Set(cacheKey, responseContent, new MemoryCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
            SlidingExpiration = TimeSpan.FromSeconds(30)
        });

        return JsonSerializer.Deserialize<TenraiAnimeResponse>(responseContent, JsonOptions) ??
            throw new TenraiApiException("Tenrai returned an empty response.");
    }

    private static string NormalizeQuery(string? query) =>
       string.IsNullOrWhiteSpace(query) ? "_" : query.Trim().ToLowerInvariant();

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
