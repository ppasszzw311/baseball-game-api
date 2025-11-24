namespace simulator_console.Models;

public class Player
{
    [System.ComponentModel.DataAnnotations.Key]
    [System.ComponentModel.DataAnnotations.Schema.Column("Id")]
    public int PlayerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public int Type { get; set; } // 0: Hitter, 1: Pitcher
    
    // 打者能力
    public int Contact { get; set; } = 30;
    public int Power { get; set; } = 15;
    public int Vision { get; set; } = 20;
    
    // 投手能力
    public int Stamina { get; set; } = 30;
    public int Control { get; set; } = 20;
    public int Velocity { get; set; } = 15;
    public int Breaking { get; set; } = 14;
    
    // 跑壘能力
    public int Speed { get; set; }
    public int BaseRunning { get; set; }
    
    // 守備能力
    public int Fielding { get; set; }
    public int Arm { get; set; }
    public int Position { get; set; }
    // 經驗
    public int Experience { get; set; }
    
    // 投手角色分類
    public int Role { get; set; } = 0;  // 0:先發 1:中繼 2:佈局 3:終結者
    
    // 投手即時狀態（不存資料庫，僅在比賽中使用）
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public double CurrentStamina { get; set; } = 100.0;  // 當前體力 (0-100)
    
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public int PitchCount { get; set; } = 0;  // 本場投球數
    
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public int ConsecutiveHitsAllowed { get; set; } = 0;  // 連續被安打數
}
