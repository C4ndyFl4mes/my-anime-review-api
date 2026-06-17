using Server.Routes.Jikan;

namespace Server.Routes.Review;

public record MarkAsHelpfulResponse
{
    public bool IsHelpful { get; set; }
}

public record MarkAsHelpfulRequest
{
    public Guid ReviewId { get; set; }
    public bool IsHelpful { get; set; }
}

public record ReviewPostRequest
{
    public string Text { get; set; } = string.Empty;
    public int Score { get; set; }
}

public record ReviewPostResponse
{
    public string Message { get; set; } = string.Empty;
}

public record ReviewResponse
{
    public Pagination Pagination { get; set; } = new();
    public List<Review> Reviews { get; set; } = [];
}

public record Review
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Score { get; set; }
    public string Username { get; set; } = string.Empty;
    public string ProfileImageURL { get; set; } = string.Empty;
    public int HelpfulCount { get; set; }
    public bool IsHelpfulByCurrentUser { get; set; }
}