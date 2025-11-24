namespace simulator_console.Models;

public class Player
{
    public int PlayerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TeamId { get; set; }
    
    // 打者能力
    public int Contract { get; set; } = 30;
    public int Power { get; set; } = 15;
    public int Discipline { get; set; } = 20;
    
    // 投手能力
    public int Stamina { get; set; } = 30;
    public int Control { get; set; } = 20;
    public int Velocity { get; set; } = 15;
    public int Movement { get; set; } = 14;
    
    // 跑壘能力
    public int Speed { get; set; }
    public int BaseRunning { get; set; }
    
    // 守備能力
    public int Fielding { get; set; }
    public int Arm { get; set; }
    public int Position { get; set; }
    // 經驗
    public int Experience { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
