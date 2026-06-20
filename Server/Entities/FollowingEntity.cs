namespace Server.Entities;

public class FollowingEntity
{
    public Guid FollowerUserId { get; set; }
    public Guid FollowedUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public required UserEntity FollowerUser { get; set; }
    public required UserEntity FollowedUser { get; set; }
}