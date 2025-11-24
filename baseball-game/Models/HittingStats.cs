using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace simulator_console.Models;

[Table("Stats_Hitting")]
public class HittingStats
{
    [Key]
    public int Id { get; set; }
    
    public int PlayerId { get; set; }
    public string SeasonId { get; set; } = string.Empty;
    
    // Plate Appearances
    public int PlateAppearances { get; set; } = 0;  // 打席數
    public int AtBats { get; set; } = 0;            // 打數
    
    // Hits
    public int Hits { get; set; } = 0;              // 安打
    public int Singles { get; set; } = 0;           // 一壘安打
    public int Doubles { get; set; } = 0;           // 二壘安打
    public int Triples { get; set; } = 0;           // 三壘安打
    public int HomeRuns { get; set; } = 0;          // 全壘打
    
    // Runs and RBIs
    public int Runs { get; set; } = 0;              // 得分
    public int RBI { get; set; } = 0;               // 打點
    
    // Walks and Strikeouts
    public int Walks { get; set; } = 0;             // 保送
    public int Strikeouts { get; set; } = 0;        // 三振
    
    // Other
    public int StolenBases { get; set; } = 0;       // 盜壘成功
    public int CaughtStealing { get; set; } = 0;    // 盜壘失敗
    
    // Calculated Stats
    public double BattingAverage { get; set; } = 0.0;     // 打擊率
    public double OnBasePercentage { get; set; } = 0.0;   // 上壘率
    public double SluggingPercentage { get; set; } = 0.0; // 長打率
    public double OPS { get; set; } = 0.0;                // 上壘+長打
    
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
        // 打擊率 = 安打 / 打數
        BattingAverage = AtBats > 0 ? (double)Hits / AtBats : 0.0;
        
        // 上壘率 = (安打 + 保送) / (打數 + 保送)
        // 簡化版本，未考慮觸身球和犧牲飛球
        int obpDenominator = AtBats + Walks;
        OnBasePercentage = obpDenominator > 0 ? (double)(Hits + Walks) / obpDenominator : 0.0;
        
        // 壘打數 = 1B + 2*2B + 3*3B + 4*HR
        int totalBases = Singles + (Doubles * 2) + (Triples * 3) + (HomeRuns * 4);
        
        // 長打率 = 壘打數 / 打數
        SluggingPercentage = AtBats > 0 ? (double)totalBases / AtBats : 0.0;
        
        // OPS = 上壘率 + 長打率
        OPS = OnBasePercentage + SluggingPercentage;
    }
}
