using simulator_console.Models;
using simulator_console.Data;

namespace simulator_console.Services.PlayerPackage;

public class PlayerSerivce
{
    private readonly GameDbContext _context;

    public PlayerSerivce(GameDbContext context)
    {
        _context = context;
    }

    public List<Player> CreateTeam(int teamId, string teamName)
    {
        // Try to fetch from DB
        var roster = _context.Players
            .Where(p => p.TeamId == teamId && p.Type == 0) // 0 for Hitter
            .ToList();

        if (roster.Any())
        {
            return roster;
        }

        // Fallback: Create fake data if DB is empty (or user hasn't seeded)
        roster = new List<Player>();
        for (int i = 0; i < 9; i++)
        {
            var batter = GetFatePlayer(PlayerType.hitter);
            batter.PlayerId = i + 1 + (teamId * 100); // Simple ID generation for fake data
            batter.TeamId = teamId;
            batter.Name = $"{teamName} Batter {i + 1}";
            roster.Add(batter);
        }
        return roster;
    }

    public Player GetFatePlayer(PlayerType type)
    {
        // For Pitchers, we might want to fetch from DB too if available
        if (type == PlayerType.pictor)
        {
            // Try to find any pitcher
            var pitcher = _context.Players.FirstOrDefault(p => p.Type == 1);
            if (pitcher != null)
            {
                return pitcher;
            }
        }

        var player = new Player();
        if (type == PlayerType.pictor)
        {
            player.Name = "Pitcher";
            player.Type = 1;
            player.Stamina += (int)GetPictorRandomNumber();
            player.Control += (int)GetPictorRandomNumber();
            player.Breaking += (int)GetPictorRandomNumber();
            player.Velocity += (int)GetPictorRandomNumber();
        }
        else
        {
            player.Type = 0;
            player.Contact += (int)GetRandomNumber();
            player.Power += (int)GetRandomNumber();
            player.Vision += (int)GetRandomNumber();
        }
        
        return player;
    }
    
    // Helper method to get a pitcher for a specific team from DB
    public Player GetPitcherForTeam(int teamId)
    {
         var pitcher = _context.Players
            .FirstOrDefault(p => p.TeamId == teamId && p.Type == 1);
        
        if (pitcher != null) return pitcher;

        // Fallback
        var p = GetFatePlayer(PlayerType.pictor);
        p.TeamId = teamId;
        p.Name = $"Pitcher Team {teamId}";
        return p;
    }

    // 幫選手的初始素質加碼
    private double GetRandomNumber()
    {
        var random = new Random();
        return (random.NextDouble() * 50) - 15;
    }

    private double GetPictorRandomNumber()
    {
        var random = new Random();
        return random.NextDouble() * 20;
    }
}
