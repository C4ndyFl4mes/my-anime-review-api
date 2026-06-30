using System.Text.Json.Serialization;
using Server.Enums;
using Server.Routes.Review;

namespace Server.Routes.Jikan;

public record JikanSearchResponse
{
    [JsonPropertyName("pagination")]
    public Pagination Pagination { get; set; } = new();
    [JsonPropertyName("data")]
    public List<AnimeSearchItem> Data { get; set; } = [];
}

public record JikanAnimeResponse
{
    [JsonPropertyName("pagination")]
    public Pagination Pagination { get; set; } = new();
    [JsonPropertyName("data")]
    public List<JikanAnimeData> Data { get; set; } = [];
}

public record JikanAnimeDetailResponse
{
    [JsonPropertyName("data")]
    public JikanAnimeData Data { get; set; } = new();
}

public record JikanAnimeData
{
    [JsonPropertyName("mal_id")]
    public int MalId { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("titles")]
    public List<JikanTitle>? Titles { get; set; }

    [JsonPropertyName("images")]
    public JikanImages? Images { get; set; }

    [JsonPropertyName("trailer")]
    public JikanTrailer? Trailer { get; set; }

    [JsonPropertyName("synopsis")]
    public string? Synopsis { get; set; }

    [JsonPropertyName("episodes")]
    public int? Episodes { get; set; }

    [JsonPropertyName("duration")]
    public string? Duration { get; set; }

    [JsonPropertyName("rating")]
    public AgeRating? Rating { get; set; }

    [JsonPropertyName("status")]
    public AiringStatus? Status { get; set; }

    [JsonPropertyName("type")]
    public AnimeType? Type { get; set; }

    [JsonPropertyName("season")]
    public Season? Season { get; set; }

    [JsonPropertyName("year")]
    public int? Year { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("aired")]
    public JikanAired? Aired { get; set; }

    [JsonPropertyName("genres")]
    public List<MalObject>? Genres { get; set; }

    [JsonPropertyName("studios")]
    public List<MalObject>? Studios { get; set; }

    [JsonPropertyName("producers")]
    public List<MalObject>? Producers { get; set; }

    [JsonPropertyName("licensors")]
    public List<MalObject>? Licensors { get; set; }

    [JsonPropertyName("themes")]
    public List<MalObject>? Themes { get; set; }

    [JsonPropertyName("demographics")]
    public List<MalObject>? Demographics { get; set; }
    public string? CurrentUserWatchStatus { get; set; }
}

public record JikanTitle
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }
}

public record JikanImages
{
    [JsonPropertyName("jpg")]
    public JikanImageSet? Jpg { get; set; }
}

public record JikanImageSet
{
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("large_image_url")]
    public string? LargeImageUrl { get; set; }
}

public record JikanTrailer
{
    [JsonPropertyName("embed_url")]
    public string? EmbedUrl { get; set; }
}

public record JikanAired
{
    [JsonPropertyName("prop")]
    public JikanAiredProp? Prop { get; set; }
    [JsonPropertyName("string")]
    public string? String { get; set; }
}

public record JikanAiredProp
{
    [JsonPropertyName("string")]
    public string? String { get; set; }
}

public record Pagination
{
    [JsonPropertyName("last_visible_page")]
    public int LastVisiblePage { get; set; }

    [JsonPropertyName("has_next_page")]
    public bool HasNextPage { get; set; }
    [JsonPropertyName("current_page")]
    public int CurrentPage { get; set; }
    [JsonPropertyName("items")]
    public Items Items { get; set; } = new();
}

public record Items
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }
}

public record Anime
{
    public int MalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Synopsis { get; set; }
    public string? ImageUrl { get; set; }
    public string? TrailerUrl { get; set; }
    public string? MalUrl { get; set; }
    public int? TotalEpisodes { get; set; }
    public string? Duration { get; set; }
    public AgeRating? AgeRating { get; set; }
    public AiringStatus? AiringStatus { get; set; }
    public AnimeType? Type { get; set; }
    public Season? Season { get; set; }
    public int? Year { get; set; }
    public string? Source { get; set; }
    public AnimeMetaData? MetaData { get; set; }
    public DateTime LastSynced { get; set; }
    public List<Review.Review> TopReviews { get; set; } = [];
    public int TotalReviews { get; set; }
    public Review.Review? CurrentUserReview { get; set; }
    public bool CanCurrentUserMakeReview { get; set; }
}

public record AnimeSearchItem
{
    public int MalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public AgeRating? AgeRating { get; set; }
    public AnimeType? Type { get; set; }
    public string? CurrentUserWatchStatus { get; set; }
    public List<MalObject> Genres { get; set; } = [];
}

public record AnimeMetaData
{
    public string? Aired { get; set; }
    public List<MalObject>? Genres { get; set; }
    public List<MalObject>? Studios { get; set; }
    public List<MalObject>? Producers { get; set; }
    public List<MalObject>? Licensors { get; set; }
    public List<MalObject>? Themes { get; set; }
    public List<MalObject>? Demographics { get; set; }
}

public record MalObject
{
    [JsonPropertyName("mal_id")]
    public int MalId { get; set; }
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}