using Microsoft.AspNetCore.SignalR;
using simulator_console.Hubs;
using simulator_console.Models;
using simulator_console.Services.PlayerPackage;

namespace simulator_console.Services.Simulator;

public class GameSimulatorSerivce
{
    private readonly PlayerSerivce _playerSerivce;
    private readonly Simulator _simulator;
    private readonly IHubContext<GameHub> _hubContext;
    private GameState _gameState;
    
    private bool _isPaused = false;
    private bool _isSimulationRunning = false;
    private CancellationTokenSource? _cancellationTokenSource;

    public GameSimulatorSerivce(PlayerSerivce playerSerivce, IHubContext<GameHub> hubContext)
    {
        _playerSerivce = playerSerivce;
        _hubContext = hubContext;
        _simulator = new Simulator();
        _gameState = new GameState();
    }

    // --- API Mode: Full Simulation ---
    public GameResult SimulateFullGame()
    {
        InitializeGame();
        var logs = new List<string>();
        logs.Add("--- GAME START ---");

        while (_gameState.CurrentInning <= 9)
        {
            RunHalfInningSync(logs);
            
            if (_gameState.IsTopInning)
            {
                _gameState.IsTopInning = false;
            }
            else
            {
                _gameState.IsTopInning = true;
                _gameState.CurrentInning++;
            }
        }
        
        logs.Add("--- GAME OVER ---");
        logs.Add($"Final Score: Away {_gameState.AwayScore} - Home {_gameState.HomeScore}");

        return new GameResult
        {
            HomeScore = _gameState.HomeScore,
            AwayScore = _gameState.AwayScore,
            Winner = _gameState.HomeScore > _gameState.AwayScore ? "Home" : "Away",
            GameLogs = logs
        };
    }

    // --- SignalR Mode: Interactive Simulation ---
    public async Task StartInteractiveGame()
    {
        if (_isSimulationRunning)
        {
            _cancellationTokenSource?.Cancel();
        }

        _cancellationTokenSource = new CancellationTokenSource();
        _isSimulationRunning = true;
        _isPaused = false;

        InitializeGame();
        await SendGameUpdate("GAME START");

        _ = Task.Run(() => RunInteractiveSimulationLoop(_cancellationTokenSource.Token));
    }

    public void PauseGame()
    {
        _isPaused = true;
    }

    public void ResumeGame()
    {
        _isPaused = false;
    }

    private async Task RunInteractiveSimulationLoop(CancellationToken token)
    {
        try
        {
            while (_gameState.CurrentInning <= 9 && !token.IsCancellationRequested)
            {
                while (_isPaused)
                {
                    await Task.Delay(500, token);
                }

                await RunHalfInningAsync(token);

                if (_gameState.IsTopInning)
                {
                    _gameState.IsTopInning = false;
                }
                else
                {
                    _gameState.IsTopInning = true;
                    _gameState.CurrentInning++;
                }
            }

            if (!token.IsCancellationRequested)
            {
                await SendGameUpdate("GAME OVER");
            }
        }
        catch (OperationCanceledException)
        {
            // Simulation stopped
        }
        finally
        {
            _isSimulationRunning = false;
        }
    }

    private void InitializeGame()
    {
        _gameState = new GameState(); // Reset state
        _gameState.AwayTeamRoster = _playerSerivce.CreateTeam(1, "Away");
        _gameState.HomeTeamRoster = _playerSerivce.CreateTeam(2, "Home");
        _gameState.AwayTeamPitcher = _playerSerivce.GetFatePlayer(PlayerType.pictor);
        _gameState.HomeTeamPitcher = _playerSerivce.GetFatePlayer(PlayerType.pictor);
    }

    // --- Sync Logic (for API) ---
    private void RunHalfInningSync(List<string> logs)
    {
        _gameState.ResetForNewHalfInning();
        string currentBattingTeamName = _gameState.IsTopInning ? "Away" : "Home";
        logs.Add($"\n--- Inning: {_gameState.CurrentInning} ({(_gameState.IsTopInning ? "Top" : "Bottom")}), Batting: {currentBattingTeamName} ---");

        while (_gameState.Outs < 3)
        {
            var (batter, pitcher) = GetCurrentMatchup();
            var result = _simulator.SimulateAtBat(batter, pitcher);
            
            logs.Add($"Batter {batter.Name} vs Pitcher {pitcher.Name}... Result: {result.ResultType}");
            ProcessAtBatResult(result, logs);
            AdvanceBatterIndex();
        }
    }

    // --- Async Logic (for SignalR) ---
    private async Task RunHalfInningAsync(CancellationToken token)
    {
        _gameState.ResetForNewHalfInning();
        await SendGameUpdate($"Inning Start: {_gameState.CurrentInning} ({(_gameState.IsTopInning ? "Top" : "Bottom")})");

        while (_gameState.Outs < 3 && !token.IsCancellationRequested)
        {
            while (_isPaused) await Task.Delay(500, token);

            var (batter, pitcher) = GetCurrentMatchup();
            var result = _simulator.SimulateAtBat(batter, pitcher);

            ProcessAtBatResult(result, null); // No logs needed for SignalR, just state update
            AdvanceBatterIndex();

            await SendGameUpdate($"Batter: {batter.Name}, Result: {result.ResultType}");
            await Task.Delay(1000, token); // Delay for visual effect
        }
    }

    private (Player batter, Player pitcher) GetCurrentMatchup()
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
        return (batter, pitcher);
    }

    private void AdvanceBatterIndex()
    {
        if (_gameState.IsTopInning)
        {
            _gameState.AwayTeamBatterIndex = (_gameState.AwayTeamBatterIndex + 1) % 9;
        }
        else
        {
            _gameState.HomeTeamBatterIndex = (_gameState.HomeTeamBatterIndex + 1) % 9;
        }
    }

    private void ProcessAtBatResult(AtBatResult result, List<string>? logs)
    {
        switch (result.ResultType)
        {
            case AtBatType.StrikeOut:
            case AtBatType.Out:
                _gameState.Outs++;
                break;
            case AtBatType.Walk:
                AdvanceRunners(1, true, result.Hitter, logs);
                break;
            case AtBatType.Single:
                AdvanceRunners(1, false, result.Hitter, logs);
                break;
            case AtBatType.Double:
                AdvanceRunners(2, false, result.Hitter, logs);
                break;
            case AtBatType.Triple:
                AdvanceRunners(3, false, result.Hitter, logs);
                break;
            case AtBatType.HomeRun:
                AdvanceRunners(4, false, result.Hitter, logs);
                break;
        }
    }

    private void AdvanceRunners(int basesToAdvance, bool isWalk, Player currentBatter, List<string>? logs)
    {
        if (isWalk)
        {
            if (_gameState.Bases[1] != null)
            {
                if (_gameState.Bases[2] != null)
                {
                    if (_gameState.Bases[3] != null)
                    {
                        ScoreRun(_gameState.Bases[3]!, logs);
                    }
                    _gameState.Bases[3] = _gameState.Bases[2];
                }
                _gameState.Bases[2] = _gameState.Bases[1];
            }
            _gameState.Bases[1] = currentBatter;
            return;
        }

        for (int i = 3; i >= 1; i--)
        {
            if (_gameState.Bases[i] != null)
            {
                Player runner = _gameState.Bases[i]!;
                int newBase = i + basesToAdvance;
                if (newBase > 3)
                {
                    ScoreRun(runner, logs);
                }
                else
                {
                    _gameState.Bases[newBase] = runner;
                }
                _gameState.Bases[i] = null;
            }
        }
        
        if (basesToAdvance > 3)
        {
            ScoreRun(currentBatter, logs);
        }
        else
        {
            _gameState.Bases[basesToAdvance] = currentBatter;
        }
    }

    private void ScoreRun(Player runner, List<string>? logs)
    {
        if (_gameState.IsTopInning)
        {
            _gameState.AwayScore++;
        }
        else
        {
            _gameState.HomeScore++;
        }
        logs?.Add($"    -> {runner.Name} scores!");
    }

    private async Task SendGameUpdate(string lastPlayResult)
    {
        var (batter, pitcher) = GetCurrentMatchup();
        
        var update = new GameUpdateDTO
        {
            CurrentInning = _gameState.CurrentInning,
            IsTopInning = _gameState.IsTopInning,
            Outs = _gameState.Outs,
            HomeScore = _gameState.HomeScore,
            AwayScore = _gameState.AwayScore,
            Bases = _gameState.Bases,
            LastPlayResult = lastPlayResult,
            CurrentBatterName = batter.Name,
            CurrentPitcherName = pitcher.Name,
            IsGameOver = _gameState.CurrentInning > 9
        };

        await _hubContext.Clients.All.SendAsync("ReceiveGameUpdate", update);
    }
}
