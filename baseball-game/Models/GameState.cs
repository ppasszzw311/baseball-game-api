using simulator_console.Services.PlayerPackage;

namespace simulator_console.Models;

public class GameState
{
    public int CurrentInning { get; set; } = 1;
    public bool IsTopInning { get; set; } = true;
    public int Outs { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }

    // 壘包狀態，索引 0 不使用，1=一壘, 2=二壘, 3=三壘
    // 儲存的是佔據壘包的球員
    public Player?[] Bases { get; set; } = new Player?[4];

    public List<Player> HomeTeamRoster { get; set; } = new();
    public List<Player> AwayTeamRoster { get; set; } = new();
    
    public Player HomeTeamPitcher { get; set; } = new();
    public Player AwayTeamPitcher { get; set; } = new();

    public int HomeTeamBatterIndex { get; set; } = 0;
    public int AwayTeamBatterIndex { get; set; } = 0;
    
    // 投手狀態追蹤
    public int HomeTeamPitcherPitchCount { get; set; } = 0;
    public int AwayTeamPitcherPitchCount { get; set; } = 0;
    public double HomeTeamPitcherStamina { get; set; } = 100.0;
    public double AwayTeamPitcherStamina { get; set; } = 100.0;
    
    // 牛棚管理
    public List<Player> HomeTeamBullpen { get; set; } = new();
    public List<Player> AwayTeamBullpen { get; set; } = new();
    
    // 本局失分追蹤
    public int HomeTeamRunsAllowedThisInning { get; set; } = 0;
    public int AwayTeamRunsAllowedThisInning { get; set; } = 0;
    
    // 累計失分
    public int HomeTeamRunsAllowed { get; set; } = 0;
    public int AwayTeamRunsAllowed { get; set; } = 0;
    
    // 換投歷史
    public List<PitcherChange> PitcherChanges { get; set; } = new();

    public void ResetForNewHalfInning()
    {
        Outs = 0;
        Bases = new Player?[4];
        // 重置本局失分
        if (IsTopInning)
            HomeTeamRunsAllowedThisInning = 0;
        else
            AwayTeamRunsAllowedThisInning = 0;
    }
}
