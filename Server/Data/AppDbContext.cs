using Microsoft.EntityFrameworkCore;
using Server.Entities;

namespace Server.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<AnimeEntity> Animes { get; set; }
    public DbSet<ReviewEntity> Reviews { get; set; }
    public DbSet<HelpfulEntity> HelpfulMarks { get; set; }

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);

        model.Entity<ReviewEntity>(e =>
        {
            e.HasKey(x => x.Id);

            e.HasOne(x => x.Anime)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.AnimeId)
                .OnDelete(DeleteBehavior.Cascade);
            
            e.HasOne(x => x.User)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            e.HasIndex(x => new { x.AnimeId, x.CreatedAt });

            e.HasIndex(x => new { x.AnimeId, x.UserId }).IsUnique();
        });

        model.Entity<HelpfulEntity>(e =>
        {
            e.HasKey(x => new { x.UserId, x.ReviewId });

            e.HasOne(x => x.User)
                .WithMany(x => x.HelpfulReviews)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            e.HasOne(x => x.Review)
                .WithMany(x => x.HelpfulByUsers)
                .HasForeignKey(x => x.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
            
            e.HasIndex(x => x.ReviewId);
        });
    }
}