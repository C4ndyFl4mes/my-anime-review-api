namespace Server.Entities;

public class ReportedUserEntity
{
    public Guid Id { get; set; }
    public Guid ReportedUserId { get; set; }
    public required string Reason { get; set; }
    public DateTime CreatedAt { get; set; }

    public required UserEntity ReportedUser { get; set; }
}