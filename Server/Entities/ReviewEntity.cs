using System.ComponentModel.DataAnnotations;

namespace Server.Entities;

public class ReviewEntity
{
    public Guid Id { get; set; }
    public int AnimeId { get; set; }
    public Guid UserId { get; set; }
    [MaxLength(4000, ErrorMessage = "A review cannot be longer than 4000 characters.")]
    public string Text { get; set; } = string.Empty;
    public int Score { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public required AnimeEntity Anime { get; set; }
    public required UserEntity User { get; set; }
    
    public ICollection<HelpfulEntity> HelpfulByUsers { get; set; } = [];
}