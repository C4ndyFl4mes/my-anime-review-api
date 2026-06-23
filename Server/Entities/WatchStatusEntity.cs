using Server.Enums;

namespace Server.Entities;

public class WatchStatusEntity
{
    public Guid UserId { get; set; }
    public int AnimeId { get; set; }
    public int EpisodesWatched { get; set; }
    public WatchStatus Status { get; set; }
    public DateTime UpdatedAt { get; set; }

    public required UserEntity User { get; set; }
    public required AnimeEntity Anime { get; set; }
}