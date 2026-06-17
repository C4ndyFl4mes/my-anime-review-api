namespace Server.Entities;

public class HelpfulEntity
{
    public Guid UserId { get; set; }
    public Guid ReviewId { get; set; }

    public required UserEntity User { get; set; }
    public required ReviewEntity Review { get; set; }
}