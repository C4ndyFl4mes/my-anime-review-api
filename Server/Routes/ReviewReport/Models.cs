using Server.Routes.Tenrai;

namespace Server.Routes.ReviewReport;

public record PostReviewReportRequest
{
    public required Guid ReportedReviewId { get; set; }
}

public record ReviewReportResponse
{
    public string Message { get; set; } = string.Empty;
}

public record GetReviewReportsResponse
{
    public Pagination Pagination { get; set; } = new();
    public IEnumerable<ReviewReport> Reports { get; set; } = [];
}

public record ReviewReport
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public required Review.Review ReportedReview { get; set; }
}