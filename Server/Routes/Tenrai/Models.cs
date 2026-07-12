using System.Text.Json.Serialization;
using Server.Enums;
using Server.Routes.Review;
using Server.Utils;

namespace Server.Routes.Tenrai;

public record TenraiSearchResponse
{
    [JsonPropertyName("pagination")]
    public Pagination Pagination { get; set; } = new();
    [JsonPropertyName("data")]
    public List<AnimeSearchItem> Data { get; set; } = [];
}

public record TenraiAnimeResponse
{
    [JsonPropertyName("pagination")]
    public Pagination Pagination { get; set; } = new();
    [JsonPropertyName("data")]
    public List<TenraiAnimeData> Data { get; set; } = [];
}

public record TenraiAnimeDetailResponse
{
    [JsonPropertyName("data")]
    public TenraiAnimeData Data { get; set; } = new();
}

public record TenraiAnimeData
{
    [JsonPropertyName("mal_id")]
    public int MalId { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("titles")]
    public List<TenraiTitle>? Titles { get; set; }

    [JsonPropertyName("images")]
    public TenraiImages? Images { get; set; }

    [JsonPropertyName("trailer")]
    public TenraiTrailer? Trailer { get; set; }

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
    public TenraiAired? Aired { get; set; }

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
    [JsonConverter(typeof(TwoDecimalNullableDoubleConverter))]
    public double? Score { get; set; }
}

public record TenraiTitle
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }
}

public record TenraiImages
{
    [JsonPropertyName("jpg")]
    public TenraiImageSet? Jpg { get; set; }
}

public record TenraiImageSet
{
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("large_image_url")]
    public string? LargeImageUrl { get; set; }
}

public record TenraiTrailer
{
    [JsonPropertyName("embed_url")]
    public string? EmbedUrl { get; set; }
}

public record TenraiAired
{
    [JsonPropertyName("prop")]
    public TenraiAiredProp? Prop { get; set; }
    [JsonPropertyName("string")]
    public string? String { get; set; }
}

public record TenraiAiredProp
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
    [JsonConverter(typeof(TwoDecimalNullableDoubleConverter))]
    public double? Score { get; set; }
}

public record AnimeSearchItem
{
    public int MalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public AgeRating? AgeRating { get; set; }
    public AnimeType? Type { get; set; }
    public string? CurrentUserWatchStatus { get; set; }
    [JsonConverter(typeof(TwoDecimalNullableDoubleConverter))]
    public double? Score { get; set; }
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
