using Server.Enums;

namespace Server.Entities;

public class ReportedBugEntity
{
    public Guid Id { get; set; }
    public BugState State { get; set; } = BugState.Pending;
    public required string Details { get; set; }
    public DateTime CreatedAt { get; set; }
}