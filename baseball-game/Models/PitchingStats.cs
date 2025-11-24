using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace simulator_console.Models;

[Table("Stats_Pitching")]
public class PitchingStats
{
    [Key]
    public int Id { get; set; }
    
    public int PlayerId { get; set; }
    public string SeasonId { get; set; } = string.Empty;
    
    // Games and Innings
    public int GamesPlayed { get; set; } = 0;       // 出賽數
    public int GamesStarted { get; set; } = 0;      // 先發場次
    public double InningsPitched { get; set; } = 0.0; // 投球局數
    
    // Outs and Batters Faced
    public int BattersFaced { get; set; } = 0;      // 面對打者數
    
    // Hits and Runs Allowed
    public int HitsAllowed { get; set; } = 0;       // 被安打
    public int RunsAllowed { get; set; } = 0;       // 失分
    public int EarnedRuns { get; set; } = 0;        // 自責分
    public int HomeRunsAllowed { get; set; } = 0;   // 被全壘打
    
    // Walks and Strikeouts
    public int WalksAllowed { get; set; } = 0;      // 保送
    public int Strikeouts { get; set; } = 0;        // 三振
    
    // Wins and Losses
    public int Wins { get; set; } = 0;              // 勝場
    public int Losses { get; set; } = 0;            // 敗場
    public int Saves { get; set; } = 0;             // 救援成功
    
    // Calculated Stats
    public double ERA { get; set; } = 0.0;          // 防禦率
    public double WHIP { get; set; } = 0.0;         // (保送+被安打)/局數
    public double StrikeoutsPer9 { get; set; } = 0.0; // K/9
    public double WalksPer9 { get; set; } = 0.0;    // BB/9
    
    // Navigation properties
    [ForeignKey("PlayerId")]
    public Player? Player { get; set; }
    
    [ForeignKey("SeasonId")]
    public Season? Season { get; set; }
    
    /// <summary>
    /// 計算並更新所有統計數據
    /// </summary>
    public void CalculateStats()
    {
        if (InningsPitched <= 0) return;
        
        // ERA = (自責分 * 9) / 投球局數
        ERA = (EarnedRuns * 9.0) / InningsPitched;
        
        // WHIP = (保送 + 被安打) / 投球局數
        WHIP = (WalksAllowed + HitsAllowed) / InningsPitched;
        
        // K/9 = (三振 * 9) / 投球局數
        StrikeoutsPer9 = (Strikeouts * 9.0) / InningsPitched;
        
        // BB/9 = (保送 * 9) / 投球局數
        WalksPer9 = (WalksAllowed * 9.0) / InningsPitched;
    }
}
