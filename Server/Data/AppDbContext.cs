using Microsoft.EntityFrameworkCore;
using Server.Entities;

namespace Server.Data;

public class AppDbContext (DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
}