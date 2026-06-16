using Server.Enums;

namespace Server.Entities;

public class AnimeEntity
{
    public required int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? TrailerUrl { get; set; }
    public string? MalUrl { get; set; }
    public string? Synopsis { get; set; }
    public AgeRating? AgeRating { get; set; }
    public AiringStatus? AiringStatus { get; set; }
    public int? TotalEpisodes { get; set; }
    public string? Duration { get; set; }
    public Season? Season { get; set; }
    public int? Year { get; set; }
    public string? Source { get; set; }
    public AnimeType? Type { get; set; }

    public string? MetaDataJSON { get; set; } // Store any additional metadata as JSON string such as Broadcast, Genres, Studios, Producers, Licensors, Themes, and Demographics, can be deserialized into an object when used.
    public DateTime LastSynced { get; set; } // Track when the anime data was last synced with Jikan API
}