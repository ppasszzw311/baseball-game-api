using simulator_console.Models;

namespace simulator_console.Services;

/// <summary>
/// 投手管理服務 - 負責投手疲勞計算、換投判定與後援投手選擇
/// </summary>
public class PitcherManager
{
    private readonly Random _random = new();
    
    /// <summary>
    /// 初始化投手體力與狀態
    /// </summary>
    public void InitializePitcher(Player pitcher)
    {
        pitcher.CurrentStamina = 100.0;
        pitcher.PitchCount = 0;
        pitcher.ConsecutiveHitsAllowed = 0;
    }
    
    /// <summary>
    /// 更新投手體力（每個打席後調用）
    /// </summary>
    public void UpdateStamina(Player pitcher, AtBatResult result, int basesOccupied)
    {
        // 計算基礎消耗
        double baseStaminaCost = 100.0 / (pitcher.Stamina * 0.8);
        
        // 根據結果調整消耗
        double multiplier = result.ResultType switch
        {
            AtBatType.StrikeOut => 1.2,    // 三振需要更多用力投球
            AtBatType.Walk => 1.5,          // 保送球數多，心理壓力大
            AtBatType.HomeRun => 2.0,       // 被全壘打重大打擊
            AtBatType.Double or AtBatType.Triple or AtBatType.Single => 1.3,  // 被安打
            _ => 1.0                        // 一般出局
        };
        
        double staminaCost = baseStaminaCost * multiplier;
        
        // 壘上有人額外消耗（牽制跑者）
        staminaCost += basesOccupied * 2.0;
        
        // 更新體力（不低於0）
        pitcher.CurrentStamina = Math.Max(0, pitcher.CurrentStamina - staminaCost);
        
        // 更新投球數（使用實際投球數：好球 + 壞球）
        int actualPitches = result.PitchCount.Strikes + result.PitchCount.Balls;
        pitcher.PitchCount += actualPitches;
        
        // 更新連續被安打
        if (result.ResultType == AtBatType.Single || 
            result.ResultType == AtBatType.Double || 
            result.ResultType == AtBatType.Triple || 
            result.ResultType == AtBatType.HomeRun)
        {
            pitcher.ConsecutiveHitsAllowed++;
        }
        else if (result.ResultType == AtBatType.Out || result.ResultType == AtBatType.StrikeOut)
        {
            pitcher.ConsecutiveHitsAllowed = 0;
        }
    }
    
    /// <summary>
    /// 檢查是否需要換投
    /// </summary>
    public PitcherChangeDecision ShouldChangePitcher(Player pitcher, GameState state)
    {
        // 極高優先級條件
        if (pitcher.CurrentStamina < 20)
            return new PitcherChangeDecision 
            { 
                ShouldChange = true, 
                Reason = PitcherChangeReason.Fatigue, 
                Priority = 1,
                Description = $"體力過低 ({pitcher.CurrentStamina:F1}%)"
            };
        
        if (pitcher.PitchCount > 100)
            return new PitcherChangeDecision 
            { 
                ShouldChange = true, 
                Reason = PitcherChangeReason.PitchCount, 
                Priority = 1,
                Description = $"投球數過多 ({pitcher.PitchCount} 球)"
            };
        
        // 高優先級條件
        int runsThisInning = state.IsTopInning 
            ? state.HomeTeamRunsAllowedThisInning 
            : state.AwayTeamRunsAllowedThisInning;
        
        if (runsThisInning >= 3)
            return new PitcherChangeDecision 
            { 
                ShouldChange = true, 
                Reason = PitcherChangeReason.RunsAllowed, 
                Priority = 2,
                Description = $"本局失分過多 ({runsThisInning} 分)"
            };
        
        if (pitcher.ConsecutiveHitsAllowed >= 3)
            return new PitcherChangeDecision 
            { 
                ShouldChange = true, 
                Reason = PitcherChangeReason.Emergency, 
                Priority = 2,
                Description = $"連續被安打 ({pitcher.ConsecutiveHitsAllowed} 支)"
            };
        
        int totalRunsAllowed = state.IsTopInning 
            ? state.HomeTeamRunsAllowed 
            : state.AwayTeamRunsAllowed;
        
        if (totalRunsAllowed >= 5)
            return new PitcherChangeDecision 
            { 
                ShouldChange = true, 
                Reason = PitcherChangeReason.RunsAllowed, 
                Priority = 2,
                Description = $"累計失分過多 ({totalRunsAllowed} 分)"
            };
        
        // 中優先級條件
        if (state.CurrentInning >= 7 && pitcher.Role == (int)PitcherRole.Starter)
            return new PitcherChangeDecision 
            { 
                ShouldChange = true, 
                Reason = PitcherChangeReason.InningLimit, 
                Priority = 3,
                Description = $"先發投手局數限制 (已投 {state.CurrentInning - 1} 局)"
            };
        
        if (pitcher.CurrentStamina < 40)
            return new PitcherChangeDecision 
            { 
                ShouldChange = true, 
                Reason = PitcherChangeReason.Fatigue, 
                Priority = 3,
                Description = $"體力偏低 ({pitcher.CurrentStamina:F1}%)"
            };
        
        return new PitcherChangeDecision { ShouldChange = false };
    }
    
    /// <summary>
    /// 選擇後援投手
    /// </summary>
    public Player? SelectReliever(List<Player> bullpen, GameState state)
    {
        if (bullpen == null || !bullpen.Any())
            return null;
        
        // 優先選擇體力充足且未上場的投手
        var availablePitchers = bullpen
            .Where(p => p.CurrentStamina >= 80)  // 體力充足
            .Where(p => p.PitchCount == 0)       // 本場未上場
            .ToList();
        
        if (!availablePitchers.Any())
        {
            // 如果沒有理想選擇，選體力最好的
            availablePitchers = bullpen
                .OrderByDescending(p => p.CurrentStamina)
                .Take(1)
                .ToList();
        }
        
        if (!availablePitchers.Any())
            return null;
        
        // 根據局數選擇適當角色
        if (state.CurrentInning >= 9 && IsLeading(state))
        {
            // 第9局領先，使用終結者
            var closer = availablePitchers
                .FirstOrDefault(p => p.Role == (int)PitcherRole.Closer);
            if (closer != null) return closer;
        }
        else if (state.CurrentInning == 8)
        {
            // 第8局，使用佈局投手
            var setupMan = availablePitchers
                .FirstOrDefault(p => p.Role == (int)PitcherRole.SetupMan);
            if (setupMan != null) return setupMan;
        }
        
        // 選擇中繼投手（能力最強者）
        return availablePitchers
            .OrderByDescending(p => p.Control + p.Velocity + p.Breaking)
            .FirstOrDefault();
    }
    
    /// <summary>
    /// 判斷是否領先
    /// </summary>
    private bool IsLeading(GameState state)
    {
        return state.IsTopInning 
            ? state.HomeScore > state.AwayScore 
            : state.AwayScore > state.HomeScore;
    }
    
    /// <summary>
    /// 應用疲勞效果到投手能力（創建臨時副本）
    /// </summary>
    public Player ApplyFatigueEffect(Player pitcher)
    {
        // 體力充沛，無影響
        if (pitcher.CurrentStamina >= 70)
        {
            return pitcher;
        }
        
        // 創建一個臨時副本，不修改原始對象
        var fatigued = new Player
        {
            PlayerId = pitcher.PlayerId,
            Name = pitcher.Name,
            TeamId = pitcher.TeamId,
            Type = pitcher.Type,
            Role = pitcher.Role,
            CurrentStamina = pitcher.CurrentStamina,
            PitchCount = pitcher.PitchCount,
            ConsecutiveHitsAllowed = pitcher.ConsecutiveHitsAllowed,
            // 複製所有屬性
            Contact = pitcher.Contact,
            Power = pitcher.Power,
            Vision = pitcher.Vision,
            Stamina = pitcher.Stamina,
            Control = pitcher.Control,
            Velocity = pitcher.Velocity,
            Breaking = pitcher.Breaking,
            Speed = pitcher.Speed,
            BaseRunning = pitcher.BaseRunning,
            Fielding = pitcher.Fielding,
            Arm = pitcher.Arm,
            Position = pitcher.Position,
            Experience = pitcher.Experience
        };
        
        // 應用疲勞效果
        if (pitcher.CurrentStamina >= 50)
        {
            // 輕度疲勞 (50-69%)
            double factor = 0.85 + (pitcher.CurrentStamina - 50) / 100.0;
            fatigued.Control = (int)(pitcher.Control * factor);
            fatigued.Breaking = (int)(pitcher.Breaking * factor);
        }
        else if (pitcher.CurrentStamina >= 30)
        {
            // 中度疲勞 (30-49%)
            double factor = 0.7 + (pitcher.CurrentStamina - 30) / 100.0;
            fatigued.Control = (int)(pitcher.Control * factor);
            fatigued.Velocity = (int)(pitcher.Velocity * 0.95);
            fatigued.Breaking = (int)(pitcher.Breaking * factor);
        }
        else
        {
            // 嚴重疲勞 (<30%)
            double factor = 0.5 + pitcher.CurrentStamina / 100.0;
            fatigued.Control = (int)(pitcher.Control * factor);
            fatigued.Velocity = (int)(pitcher.Velocity * 0.85);
            fatigued.Breaking = (int)(pitcher.Breaking * factor);
        }
        
        return fatigued;
    }
}
