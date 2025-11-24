namespace simulator_console.Models;

/// <summary>
/// 投手角色枚舉
/// </summary>
public enum PitcherRole
{
    Starter = 0,          // 先發投手（1-5號）
    MiddleReliever = 1,   // 中繼投手（6-7局）
    SetupMan = 2,         // 佈局投手（8局）
    Closer = 3            // 終結者（9局救援）
}

/// <summary>
/// 換投原因枚舉
/// </summary>
public enum PitcherChangeReason
{
    Fatigue,              // 疲勞
    PitchCount,           // 投球數過多
    RunsAllowed,          // 失分過多
    InningLimit,          // 局數限制
    Emergency,            // 緊急狀況（連續被安打）
    Strategic             // 戰術換投
}

/// <summary>
/// 換投記錄
/// </summary>
public class PitcherChange
{
    public int Inning { get; set; }
    public bool IsTopInning { get; set; }
    public string OldPitcherName { get; set; } = string.Empty;
    public string NewPitcherName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// 換投決策
/// </summary>
public class PitcherChangeDecision
{
    public bool ShouldChange { get; set; }
    public PitcherChangeReason Reason { get; set; }
    public int Priority { get; set; }  // 1=最高, 3=最低
    public string Description { get; set; } = string.Empty;
}
