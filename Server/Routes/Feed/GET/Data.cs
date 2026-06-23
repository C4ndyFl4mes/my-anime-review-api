using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Enums;
using Server.Routes.Review;

namespace Server.Routes.Feed.GET;

public class GetFeedData(AppDbContext ctx)
{
    public async Task<FeedResponse> GetFeedAsync(Guid currentUserId, int page, int pageSize, CancellationToken ct)
    {
        int safePageSize = Math.Clamp(pageSize <= 0 ? 20 : pageSize, 1, 50);

        HashSet<Guid> followedUserIds = await ctx.FollowInstances
            .AsNoTracking()
            .Where(f => f.FollowerUserId == currentUserId)
            .Select(f => f.FollowedUserId)
            .ToHashSetAsync(ct);

        if (followedUserIds.Count == 0)
        {
            return new()
            {
                Pagination = new()
                {
                    CurrentPage = 1,
                    LastVisiblePage = 1,
                    HasNextPage = false,
                    Items = new()
                    {
                        Count = 0,
                        Total = 0,
                        PerPage = safePageSize
                    }
                },
                FeedItems = []
            };
        }

        List<FeedEventRow> reviewEvents = await ctx.Reviews
            .AsNoTracking()
            .Where(r => followedUserIds.Contains(r.UserId))
            .Select(r => new FeedEventRow
            {
                Type = FeedEventType.ReviewPosted,
                EventId = "review:" + r.Id,
                CreatedAt = r.CreatedAt,
                ActorUsername = r.User.Username,
                ActorProfileImageURL = r.User.ProfileImageURL,

                ReviewId = r.Id,
                ReviewUserId = r.UserId,
                ReviewText = r.Text,
                ReviewScore = r.Score,
                ReviewUsername = r.User.Username,
                ReviewProfileImageURL = r.User.ProfileImageURL,
                ReviewCreatedAt = r.CreatedAt,
                ReviewUpdatedAt = r.UpdatedAt,
                HelpfulCount = r.HelpfulByUsers.Count,
                IsHelpfulByCurrentUser = r.HelpfulByUsers.Any(h => h.UserId == currentUserId),

                AnimeId = r.AnimeId,
                AnimeTitle = r.Anime.Title,
                AnimeImageUrl = r.Anime.ImageUrl,
                AnimeAgeRating = r.Anime.AgeRating,
                AnimeType = r.Anime.Type
            })
            .ToListAsync(ct);

        List<FeedEventRow> helpfulEvents = await ctx.HelpfulMarks
            .AsNoTracking()
            .Where(h => followedUserIds.Contains(h.UserId))
            .Select(h => new FeedEventRow
            {
                Type = FeedEventType.ReviewMarkedHelpful,
                EventId = "helpful:" + h.UserId + ":" + h.ReviewId,
                CreatedAt = h.CreatedAt,
                ActorUsername = h.User.Username,
                ActorProfileImageURL = h.User.ProfileImageURL,

                ReviewId = h.ReviewId,
                ReviewUserId = h.Review.UserId,
                ReviewText = h.Review.Text,
                ReviewScore = h.Review.Score,
                ReviewUsername = h.Review.User.Username,
                ReviewProfileImageURL = h.Review.User.ProfileImageURL,
                ReviewCreatedAt = h.Review.CreatedAt,
                ReviewUpdatedAt = h.Review.UpdatedAt,
                HelpfulCount = h.Review.HelpfulByUsers.Count,
                IsHelpfulByCurrentUser = h.Review.HelpfulByUsers.Any(x => x.UserId == currentUserId),

                AnimeId = h.Review.AnimeId,
                AnimeTitle = h.Review.Anime.Title,
                AnimeImageUrl = h.Review.Anime.ImageUrl,
                AnimeAgeRating = h.Review.Anime.AgeRating,
                AnimeType = h.Review.Anime.Type
            })
            .ToListAsync(ct);

        List<FeedEventRow> completedEvents = await ctx.WatchStatuses
            .AsNoTracking()
            .Where(ws => followedUserIds.Contains(ws.UserId) && ws.Status == Enums.WatchStatus.Completed)
            .Select(ws => new FeedEventRow
            {
                Type = FeedEventType.AnimeCompleted,
                EventId = "completed:" + ws.UserId + ":" + ws.AnimeId,
                CreatedAt = ws.UpdatedAt,
                ActorUsername = ws.User.Username,
                ActorProfileImageURL = ws.User.ProfileImageURL,

                AnimeId = ws.AnimeId,
                AnimeTitle = ws.Anime.Title,
                AnimeImageUrl = ws.Anime.ImageUrl,
                AnimeAgeRating = ws.Anime.AgeRating,
                AnimeType = ws.Anime.Type
            })
            .ToListAsync(ct);

        List<FeedEventRow> allEvents = reviewEvents
            .Concat(helpfulEvents)
            .Concat(completedEvents)
            .OrderByDescending(e => e.CreatedAt)
            .ThenByDescending(e => e.EventId)
            .ToList();

        int total = allEvents.Count;
        int lastVisiblePage = total == 0 ? 1 : (int)Math.Ceiling(total / (double)safePageSize);
        int safePage = Math.Clamp(page < 1 ? 1 : page, 1, lastVisiblePage);

        List<FeedItemBase> items = allEvents
            .Skip((safePage - 1) * safePageSize)
            .Take(safePageSize)
            .Select(MapToFeedItem)
            .ToList();

        return new()
        {
            Pagination = new()
            {
                CurrentPage = safePage,
                LastVisiblePage = lastVisiblePage,
                HasNextPage = safePage < lastVisiblePage,
                Items = new()
                {
                    Count = items.Count,
                    Total = total,
                    PerPage = safePageSize
                }
            },
            FeedItems = items
        };
    }

    private static FeedItemBase MapToFeedItem(FeedEventRow e)
    {
        if (e.Type == FeedEventType.ReviewPosted)
        {
            return new ReviewPostedFeedItem
            {
                EventId = e.EventId,
                CreatedAt = e.CreatedAt,
                Username = e.ActorUsername,
                ProfileImageURL = e.ActorProfileImageURL,
                Review = BuildReviewExtended(e)
            };
        }

        if (e.Type == FeedEventType.ReviewMarkedHelpful)
        {
            return new ReviewMarkedHelpfulFeedItem
            {
                EventId = e.EventId,
                CreatedAt = e.CreatedAt,
                Username = e.ActorUsername,
                ProfileImageURL = e.ActorProfileImageURL,
                MarkedHelpfulReview = BuildReviewExtended(e)
            };
        }

        return new AnimeCompletedFeedItem
        {
            EventId = e.EventId,
            CreatedAt = e.CreatedAt,
            Username = e.ActorUsername,
            ProfileImageURL = e.ActorProfileImageURL,
            Anime = new()
            {
                MalId = e.AnimeId,
                Title = e.AnimeTitle ?? string.Empty,
                ImageUrl = e.AnimeImageUrl,
                AgeRating = e.AnimeAgeRating,
                Type = e.AnimeType,
                Genres = []
            }
        };
    }

    private static ReviewExtended BuildReviewExtended(FeedEventRow e)
    {
        return new()
        {
            AnimeId = e.AnimeId,
            Title = e.AnimeTitle ?? string.Empty,
            Review = new Review.Review
            {
                Id = e.ReviewId ?? Guid.Empty,
                UserId = e.ReviewUserId ?? Guid.Empty,
                Text = e.ReviewText ?? string.Empty,
                Score = e.ReviewScore ?? 0,
                Username = e.ReviewUsername ?? string.Empty,
                ProfileImageURL = e.ReviewProfileImageURL ?? string.Empty,
                    CreatedAt = e.ReviewCreatedAt ?? e.CreatedAt,
                    UpdatedAt = e.ReviewUpdatedAt,
                HelpfulCount = e.HelpfulCount ?? 0,
                IsHelpfulByCurrentUser = e.IsHelpfulByCurrentUser ?? false
            }
        };
    }

    private enum FeedEventType
    {
        ReviewPosted,
        ReviewMarkedHelpful,
        AnimeCompleted
    }

    private sealed record FeedEventRow
    {
        public required FeedEventType Type { get; set; }
        public required string EventId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required string ActorUsername { get; set; }
        public required string ActorProfileImageURL { get; set; }

        public Guid? ReviewId { get; set; }
        public Guid? ReviewUserId { get; set; }
        public string? ReviewText { get; set; }
        public int? ReviewScore { get; set; }
        public string? ReviewUsername { get; set; }
        public string? ReviewProfileImageURL { get; set; }
        public DateTime? ReviewCreatedAt { get; set; }
        public DateTime? ReviewUpdatedAt { get; set; }
        public int? HelpfulCount { get; set; }
        public bool? IsHelpfulByCurrentUser { get; set; }

        public int AnimeId { get; set; }
        public string? AnimeTitle { get; set; }
        public string? AnimeImageUrl { get; set; }
        public AgeRating? AnimeAgeRating { get; set; }
        public AnimeType? AnimeType { get; set; }
    }
}