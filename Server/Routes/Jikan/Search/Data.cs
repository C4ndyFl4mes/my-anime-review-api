using System.Text.Json;
using System.Text.Json.Serialization;
using Server.Exceptions;

namespace Server.Routes.Jikan.Search;

/// <summary>
/// Data class for searching anime using Jikan API. Supports optional query parameter 'q' for search term and 'page' for pagination.
/// </summary>
public class JikanSearchData
{
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
    public async Task<JikanSearchResponse> SearchAnimeAsync(string? query, int page, CancellationToken ct)
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

        try
        {
            JikanAnimeResponse payload = JsonSerializer.Deserialize<JikanAnimeResponse>(responseContent, JsonOptions) ??
                throw new JikanApiException("Jikan returned an empty response.");

            return new JikanSearchResponse
            {
                Pagination = payload.Pagination,
                Data = payload.Data.Select(a => a.ToAnimeSearchItem()).ToList()
            };
        }
        catch (JsonException ex)
        {
            throw new JikanApiException($"Failed to parse Jikan response: {ex.Message}");
        }
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