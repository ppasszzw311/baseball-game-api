namespace simulator_console.Models;

/// <summary>
/// 擊球品質類型（用於守備判定）
/// </summary>
public enum BallQuality
{
    None,           // 未擊球（三振、保送等）
    WeakGroundBall, // 軟弱滾地球
    GroundBall,     // 滾地球
    LineDrive,      // 強勁平飛球
    FlyBall,        // 飛球
    DeepFlyBall,    // 深遠飛球
    Bomb            // 強力長打（幾乎確定安打或全壘打）
}

/// <summary>
/// 對戰結果
/// </summary>
public enum AtBatType
{
    StrikeOut, // 三振
    Walk, // 保送
    Single, // 一壘安打
    Double, // 二壘安打
    Triple, // 三壘安打
    HomeRun, // 全壘打
    Out, // 單純的出局
    Foul, // 界外球
    SwingMiss // 揮空
}

// 對戰結果
public class AtBatResult
{
    public AtBatType ResultType { get; set; } // 結果類型
    public BallQuality Quality { get; set; } = BallQuality.None; // 擊球品質（供守備系統使用）
    public required Player Pitcher { get; set; } // 投手
    public required Player Hitter { get; set; } // 打者
    public bool IsOut => ResultType == AtBatType.Out || ResultType == AtBatType.StrikeOut; // 是否出局
    public bool IsHit => ResultType == AtBatType.Single || ResultType == AtBatType.Double || ResultType == AtBatType.Triple || ResultType == AtBatType.HomeRun; // 是否安打
    public bool IsWalk => ResultType == AtBatType.Walk; // 是否保送
    public bool IsBallInPlay => Quality != BallQuality.None; // 是否有擊球出去（需要守備處理）
    public int RunScore {get; set;} // 得分數
    public PitchCount PitchCount { get; set; } = new PitchCount(); // 投球數
}

public class PitchCount
{
    public int Strikes { get; set; } // 好球數
    public int Balls { get; set; } // 壞球數
}