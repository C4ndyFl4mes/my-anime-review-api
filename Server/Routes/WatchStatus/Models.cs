using Server.Routes.Jikan;

namespace Server.Routes.WatchStatus;

public record PostWatchStatusRequest
{
    public int EpisodesWatched { get; set; }
    public Enums.WatchStatus Status { get; set; }
}

public record WatchStatusResponse
{
    public string Message { get; set; } = string.Empty;
}

public record GetWatchStatusAnimeResponse
{
    public IEnumerable<AnimeSearchItemExtended> AnimeItems { get; set; } = [];
}

public record AnimeSearchItemExtended
{
    public required AnimeSearchItem Item { get; set; }
    public int EpisodesWatched { get; set; }
    public string Status { get; set; } = string.Empty;
}