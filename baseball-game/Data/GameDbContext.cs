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
    public DbSet<Season> Seasons { get; set; }
    public DbSet<TeamEntity> Teams { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<HittingStats> HittingStats { get; set; }
    public DbSet<PitchingStats> PitchingStats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>().ToTable("Players");
        modelBuilder.Entity<GameRecord>().ToTable("GameRecords");
        modelBuilder.Entity<GameLog>().ToTable("GameLogs");
        modelBuilder.Entity<HittingStats>().ToTable("Stats_Hitting");
        modelBuilder.Entity<PitchingStats>().ToTable("Stats_Pitching");
    }
}
