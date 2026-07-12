using System.Text.Json.Serialization;
using Server.Routes.Tenrai;
using Server.Routes.Review;

namespace Server.Routes.Feed;

public record FeedResponse
{
    public Pagination Pagination { get; set; } = new();
    public List<FeedItemBase> FeedItems { get; set; } = [];
}

// FeedItem can represent either a review, marked as helpful, or a new completed anime, all comming from users that the current user is following.
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(ReviewPostedFeedItem), "review_posted")]
[JsonDerivedType(typeof(AnimeCompletedFeedItem), "anime_completed")]
[JsonDerivedType(typeof(ReviewMarkedHelpfulFeedItem), "review_marked_helpful")]
public abstract record FeedItemBase
{
    public required string EventId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string Username { get; set; }
    public required string ProfileImageURL { get; set; }
}

public sealed record ReviewPostedFeedItem : FeedItemBase
{
    public required ReviewExtended Review { get; set; } // The review that the one user is following posted.
}

public sealed record AnimeCompletedFeedItem : FeedItemBase
{
    public required AnimeSearchItem Anime { get; set; } // The anime that the one user is following completed.
}

public sealed record ReviewMarkedHelpfulFeedItem : FeedItemBase
{
    public required ReviewExtended MarkedHelpfulReview { get; set; } // The review that the one user is following marked as helpful.
}


