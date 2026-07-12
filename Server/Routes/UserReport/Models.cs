using Server.Routes.Tenrai;

namespace Server.Routes.UserReport;

public record PostUserReportRequest
{
    public Guid ReportedUserId { get; set; }
    public required string Reason { get; set; }
}

public record UserReportResponse
{
    public string Message { get; set; } = string.Empty;
}

public record GetUserReportsResponse
{
    public Pagination Pagination { get; set; } = new();
    public IEnumerable<UserReport> Reports { get; set; } = [];
}

public record UserReport
{
    public Guid Id { get; set; }
    public Guid ReportedUserId { get; set; }
    public required string Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}
