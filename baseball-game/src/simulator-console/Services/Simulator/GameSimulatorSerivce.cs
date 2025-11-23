using simulator_console.Models;
using simulator_console.Services.PlayerPackage;

namespace simulator_console.Services.Simulator;

public class GameSimulatorSerivce
{
    private readonly PlayerSerivce _playerSerivce;
    private readonly Simulator _simulator;
    private readonly GameState _gameState;

    public GameSimulatorSerivce(PlayerSerivce playerSerivce)
    {
        _playerSerivce = playerSerivce;
        _simulator = new Simulator();
        _gameState = new GameState();
    }

    public void RunSimulation()
    {
        InitializeGame();
        
        Console.WriteLine("--- GAME START ---");
        DisplayScore();

        while (_gameState.CurrentInning <= 9)
        {
            RunHalfInning();
            
            // 如果是上半局結束，輪到下半局
            if (_gameState.IsTopInning)
            {
                _gameState.IsTopInning = false;
            }
            // 如果是下半局結束，進入下一局
            else
            {
                _gameState.IsTopInning = true;
                _gameState.CurrentInning++;
            }
        }
        
        Console.WriteLine("--- GAME OVER ---");
        DisplayScore();
    }

    private void InitializeGame()
    {
        _gameState.AwayTeamRoster = _playerSerivce.CreateTeam(1, "Away");
        _gameState.HomeTeamRoster = _playerSerivce.CreateTeam(2, "Home");
        _gameState.AwayTeamPitcher = _playerSerivce.GetFatePlayer(PlayerType.pictor);
        _gameState.HomeTeamPitcher = _playerSerivce.GetFatePlayer(PlayerType.pictor);
    }

    private void RunHalfInning()
    {
        _gameState.ResetForNewHalfInning();
        
        string currentBattingTeamName = _gameState.IsTopInning ? "Away" : "Home";
        Console.WriteLine($"\n--- Inning: {_gameState.CurrentInning} ({(_gameState.IsTopInning ? "Top" : "Bottom")}), Batting: {currentBattingTeamName} ---");

        while (_gameState.Outs < 3)
        {
            Player batter;
            Player pitcher;
            int batterIndex;

            if (_gameState.IsTopInning)
            {
                batterIndex = _gameState.AwayTeamBatterIndex;
                batter = _gameState.AwayTeamRoster[batterIndex];
                pitcher = _gameState.HomeTeamPitcher;
            }
            else
            {
                batterIndex = _gameState.HomeTeamBatterIndex;
                batter = _gameState.HomeTeamRoster[batterIndex];
                pitcher = _gameState.AwayTeamPitcher;
            }

            var result = _simulator.SimulateAtBat(batter, pitcher);
            Console.WriteLine($"Batter {batter.Name} vs Pitcher {pitcher.Name}... Result: {result.ResultType}");
            
            ProcessAtBatResult(result);
            
            // 更新下一位打者索引
            if (_gameState.IsTopInning)
            {
                _gameState.AwayTeamBatterIndex = (_gameState.AwayTeamBatterIndex + 1) % 9;
            }
            else
            {
                _gameState.HomeTeamBatterIndex = (_gameState.HomeTeamBatterIndex + 1) % 9;
            }
            DisplayScore();
        }
    }

    private void ProcessAtBatResult(AtBatResult result)
    {
        switch (result.ResultType)
        {
            case AtBatType.StrikeOut:
            case AtBatType.Out:
                _gameState.Outs++;
                break;
            case AtBatType.Walk:
                AdvanceRunners(1, true, result.Hitter);
                break;
            case AtBatType.Single:
                AdvanceRunners(1, false, result.Hitter);
                break;
            case AtBatType.Double:
                AdvanceRunners(2, false, result.Hitter);
                break;
            case AtBatType.Triple:
                AdvanceRunners(3, false, result.Hitter);
                break;
            case AtBatType.HomeRun:
                AdvanceRunners(4, false, result.Hitter);
                break;
        }
    }

    private void AdvanceRunners(int basesToAdvance, bool isWalk, Player currentBatter)
    {
        // 處理保送的特殊情況
        if (isWalk)
        {
            // 檢查一壘是否有人，若有則推進，連鎖反應
            if (_gameState.Bases[1] != null)
            {
                if (_gameState.Bases[2] != null)
                {
                    if (_gameState.Bases[3] != null)
                    {
                        // 滿壘保送，三壘跑者得分
                        ScoreRun(_gameState.Bases[3]!);
                    }
                    _gameState.Bases[3] = _gameState.Bases[2];
                }
                _gameState.Bases[2] = _gameState.Bases[1];
            }
            _gameState.Bases[1] = currentBatter;
            return;
        }

        // 處理安打的情況 (從三壘開始處理避免覆蓋)
        for (int i = 3; i >= 1; i--)
        {
            if (_gameState.Bases[i] != null)
            {
                Player runner = _gameState.Bases[i]!;
                int newBase = i + basesToAdvance;
                if (newBase > 3)
                {
                    ScoreRun(runner);
                }
                else
                {
                    _gameState.Bases[newBase] = runner;
                }
                _gameState.Bases[i] = null;
            }
        }
        
        // 處理打者
        if (basesToAdvance > 3)
        {
            ScoreRun(currentBatter);
        }
        else
        {
            _gameState.Bases[basesToAdvance] = currentBatter;
        }
    }

    private void ScoreRun(Player runner)
    {
        if (_gameState.IsTopInning)
        {
            _gameState.AwayScore++;
        }
        else
        {
            _gameState.HomeScore++;
        }
        Console.WriteLine($"    -> {runner.Name} scores!");
    }

    private void DisplayScore()
    {
        Console.WriteLine($"Score: Away {_gameState.AwayScore} - Home {_gameState.HomeScore} | Outs: {_gameState.Outs}");
    }
}
