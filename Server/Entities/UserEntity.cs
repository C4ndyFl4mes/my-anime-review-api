namespace Server.Entities;

public class UserEntity
{
    public required Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string ProfileImageURL { get; set; } = string.Empty;

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public Guid RoleId { get; set; }
    public required RoleEntity Role { get; set; }

    public ICollection<ReviewEntity> Reviews { get; set; } = [];
    public ICollection<HelpfulEntity> HelpfulReviews { get; set; } = [];
    
    public ICollection<FollowingEntity> Following { get; set; } = [];
    public ICollection<FollowingEntity> Followers { get; set; } = [];
    
    public ICollection<ReportedUserEntity> Reports { get; set; } = [];

    public ICollection<WatchStatusEntity> WatchStatuses { get; set; } = [];
}