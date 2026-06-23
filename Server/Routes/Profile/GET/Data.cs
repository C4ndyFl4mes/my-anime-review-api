using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;
using Server.Routes.Review;

namespace Server.Routes.Profile.GET;

public class GetProfileData(AppDbContext ctx)
{
    public async Task<GetProfileResponse> GetProfileAsync(Guid targetUserId, Guid currentUserId, CancellationToken ct)
    {
        UserEntity user = await ctx.Users.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == targetUserId, ct) ??
            throw new NotFoundException("The user doesn't exist.");

        bool isFollowedByCurrentUser = await ctx.FollowInstances.AnyAsync(fi => fi.FollowedUserId == targetUserId && fi.FollowerUserId == currentUserId, ct);

        Dictionary<string, int> animeStats = await ctx.WatchStatuses
            .Where(w => w.UserId == targetUserId)
            .GroupBy(w => w.Status.ToString())
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, ct);
        
        
        foreach (string status in Enum.GetNames<Enums.WatchStatus>())
        {
            animeStats.TryAdd(status, 0);
        }
        animeStats.TryAdd("TotalEntries", animeStats.Values.Sum());

        Dictionary<string, int> userStats = new()
        {
            { "TotalReviews", user.Reviews.Count },
            { "TotalHelpfulMarks", await ctx.HelpfulMarks.CountAsync(h => h.Review.UserId == targetUserId, ct) },
            { "TotalFollowers", user.Followers.Count },
            { "TotalFollowing", user.Following.Count },
            { "MeanScore", user.Reviews.Count != 0 ? (int)user.Reviews.Average(r => r.Score) : 0 }
        };

        List<ReviewExtended> topReviews = await ctx.Reviews
            .AsNoTracking()
            .Include(r => r.Anime)
            .Where(r => r.UserId == targetUserId)
            .OrderByDescending(r => r.HelpfulByUsers.Count)
            .Take(3)
            .Select(r => new ReviewExtended
            {
                Review = new()
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Text = r.Text,
                    Score = r.Score,
                    Username = user.Username,
                    ProfileImageURL = user.ProfileImageURL,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    HelpfulCount = r.HelpfulByUsers.Count,
                    IsHelpfulByCurrentUser = currentUserId != Guid.Empty && r.HelpfulByUsers.Any(h => h.UserId == currentUserId)
                },
                AnimeId = r.AnimeId,
                Title = r.Anime.Title
            })
            .ToListAsync(ct);

        return new()
        {
            Username = user.Username,
            ProfileImageURL = user.ProfileImageURL,
            JoinedDate = user.CreatedAt.ToString("yyyy-MM-dd"),
            Role = user.Role.Name,
            IsFollowedByCurrentUser = isFollowedByCurrentUser,
            UserStats = userStats,
            AnimeStats = animeStats,
            TopReviews = topReviews
        };
    }
}