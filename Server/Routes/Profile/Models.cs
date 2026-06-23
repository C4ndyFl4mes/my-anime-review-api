using Server.Routes.Review;

namespace Server.Routes.Profile;

public record GetProfileResponse
{
    public string Username { get; set; } = string.Empty;
    public string ProfileImageURL { get; set; } = string.Empty;
    public string JoinedDate { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsFollowedByCurrentUser { get; set; }
    public Dictionary<string, int> UserStats { get; set; } = [];
    public Dictionary<string, int> AnimeStats { get; set; } = [];
    public List<ReviewExtended> TopReviews { get; set; } = [];
}

public record ChangeProfileImageRequest
{
    public string ProfileImageURL { get; set; } = string.Empty;
}

public record ChangeProfileImageResponse
{
    public string ProfileImageURL { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}