namespace simulator_console.Models;

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
    public Player Pitcher { get; set; } // 投手
    public Player Hitter { get; set; } // 打者
    public bool IsOut => ResultType == AtBatType.Out || ResultType == AtBatType.StrikeOut; // 是否出局
    public bool IsHit => ResultType == AtBatType.Single || ResultType == AtBatType.Double || ResultType == AtBatType.Triple || ResultType == AtBatType.HomeRun; // 是否安打
    public bool IsWalk => ResultType == AtBatType.Walk; // 是否保送
    public int RunScore {get; set;} // 得分數
    public PitchCount PitchCount { get; set; } = new PitchCount(); // 投球數
}

public class PitchCount
{
    public int Strikes { get; set; } // 好球數
    public int Balls { get; set; } // 壞球數
}