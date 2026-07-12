using Server.Enums;
using Server.Routes.Tenrai;

namespace Server.Routes.BugReport;

public record PostBugReportRequest
{
    public required string Details { get; set; }
}

public record BugReportMessageResponse
{
    public string Message { get; set; } = string.Empty;
}

public record GetBugReportsResponse
{
    public Pagination Pagination { get; set; } = new();
    public IEnumerable<BugReport> Reports { get; set; } = [];
}

public record BugReport
{
    public Guid Id { get; set; }
    public BugState State { get; set; }
    public required string Details { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record ChangeStateResponse
{
    public required string NewState { get; set; }
    public string Message { get; set; } = string.Empty;
}
