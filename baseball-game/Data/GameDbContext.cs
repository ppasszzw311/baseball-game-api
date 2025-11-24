using Microsoft.EntityFrameworkCore;
using simulator_console.Models;

namespace simulator_console.Data;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }

    public DbSet<Player> Players { get; set; }
    public DbSet<GameRecord> GameRecords { get; set; }
    public DbSet<GameLog> GameLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>().ToTable("Players");
        modelBuilder.Entity<GameRecord>().ToTable("GameRecords");
        modelBuilder.Entity<GameLog>().ToTable("GameLogs");
    }
}
