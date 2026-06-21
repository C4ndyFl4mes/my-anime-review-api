namespace Server.Entities;

public class ReportedReviewEntity {
    public Guid Id { get; set; }
    public Guid ReportedReviewId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public required ReviewEntity ReportedReview { get; set; }
}