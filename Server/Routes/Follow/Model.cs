namespace Server.Routes.Follow;

public record FollowPostRequest
{
    public Guid ToFollowUserId { get; set; }
    public bool IsFollowing { get; set; }
}

public record FollowPostResponse
{
    public string Message { get; set; } = string.Empty;
    public bool IsFollowing { get; set; }
}