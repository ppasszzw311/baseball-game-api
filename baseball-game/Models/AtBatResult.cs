namespace baseball_game.Models;

/// <summary>
/// 對戰結果
/// </summary>
public enum AtBatResult
{
    StrikeOut, // 三振
    Walk, // 保送
    Single, // 一壘安打
    Double, // 二壘安打
    Triple, // 三壘安打
    HomeRun, // 全壘打
    Out // 單純的出局
}