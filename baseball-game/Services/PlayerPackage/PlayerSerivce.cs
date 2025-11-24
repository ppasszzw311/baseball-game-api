using simulator_console.Models;
namespace simulator_console.Services.PlayerPackage;

public class PlayerSerivce
{
    public List<Player> CreateTeam(int teamId, string teamName)
    {
        var roster = new List<Player>();
        for (int i = 0; i < 9; i++)
        {
            var batter = GetFatePlayer(PlayerType.hitter);
            batter.PlayerId = i + 1;
            batter.TeamId = teamId;
            batter.Name = $"{teamName} Batter {i + 1}";
            roster.Add(batter);
        }
        return roster;
    }

    // demo 用
    public Player GetFatePlayer(PlayerType  type)
    {
        var player = new Player();
        if (type == PlayerType.pictor)
        {
            player.Name = "Pitcher";
            player.Stamina += (int)GetPictorRandomNumber();
            player.Control += (int)GetPictorRandomNumber();
            player.Movement += (int)GetPictorRandomNumber();
            player.Velocity += (int)GetPictorRandomNumber();
        }
        else
        {
            player.Contract += (int)GetRandomNumber();
            player.Power += (int)GetRandomNumber();
            player.Discipline += (int)GetRandomNumber();
        }
        
        return player;
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
