using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using simulator_console.Hubs;
using simulator_console.Models;
using simulator_console.Services.PlayerPackage;
using simulator_console.Data;

namespace simulator_console.Services.Simulator;

public class GameSimulatorSerivce
{
    private readonly PlayerSerivce _playerSerivce;
    private readonly Simulator _simulator;
    private readonly IHubContext<GameHub> _hubContext;
    private readonly GameDbContext _dbContext;
    private readonly PitcherManager _pitcherManager;
    private GameState _gameState;
    
    private bool _isPaused = false;
    private bool _isSimulationRunning = false;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly IHostApplicationLifetime _appLifetime;

    public GameSimulatorSerivce(PlayerSerivce playerSerivce, IHubContext<GameHub> hubContext, GameDbContext dbContext, IHostApplicationLifetime appLifetime)
    {
        _playerSerivce = playerSerivce;
        _hubContext = hubContext;
        _dbContext = dbContext;
        _simulator = new Simulator();
        _pitcherManager = new PitcherManager();
        _gameState = new GameState();
        _appLifetime = appLifetime;
        _appLifetime.ApplicationStopping.Register(OnShutdown);
    }

    private void OnShutdown()
    {
        _cancellationTokenSource?.Cancel();
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

        var result = new GameResult
        {
            HomeScore = _gameState.HomeScore,
            AwayScore = _gameState.AwayScore,
            Winner = _gameState.HomeScore > _gameState.AwayScore ? "Home" : "Away",
            GameLogs = logs
        };

        SaveGameResult(result);
        return result;
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
                // Save result for interactive game too
                var result = new GameResult
                {
                    HomeScore = _gameState.HomeScore,
                    AwayScore = _gameState.AwayScore,
                    Winner = _gameState.HomeScore > _gameState.AwayScore ? "Home" : "Away",
                    GameLogs = new List<string> { "Interactive Game Finished" } // We don't have full logs here easily unless we tracked them
                };
                SaveGameResult(result);
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
        _gameState.AwayTeamPitcher = _playerSerivce.GetPitcherForTeam(1);
        _gameState.HomeTeamPitcher = _playerSerivce.GetPitcherForTeam(2);
        
        // 初始化投手體力
        _pitcherManager.InitializePitcher(_gameState.AwayTeamPitcher);
        _pitcherManager.InitializePitcher(_gameState.HomeTeamPitcher);
        
        // 初始化牛棚（載入後援投手）
        _gameState.AwayTeamBullpen = GetBullpenPitchers(_gameState.AwayTeamRoster);
        _gameState.HomeTeamBullpen = GetBullpenPitchers(_gameState.HomeTeamRoster);
    }
    
    private List<Player> GetBullpenPitchers(List<Player> roster)
    {
        return roster
            .Where(p => p.Type == 1) // 投手
            .Where(p => p.Role != (int)PitcherRole.Starter) // 非先發
            .ToList();
    }

    private void SaveGameResult(GameResult result)
    {
        try
        {
            var record = new GameRecord
            {
                HomeScore = result.HomeScore,
                AwayScore = result.AwayScore,
                Winner = result.Winner,
                PlayedAt = DateTime.Now
            };
            _dbContext.GameRecords.Add(record);
            _dbContext.SaveChanges(); // Save to get Id

            if (result.GameLogs != null)
            {
                foreach (var log in result.GameLogs)
                {
                    _dbContext.GameLogs.Add(new GameLog
                    {
                        GameId = record.Id,
                        LogMessage = log
                    });
                }
                _dbContext.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving game result: {ex.Message}");
        }
    }

    // --- Sync Logic (for API) ---
    private void RunHalfInningSync(List<string> logs)
    {
        // 半局開始前檢查是否需要換投
        CheckAndChangePitcher(logs);
        
        _gameState.ResetForNewHalfInning();
        string currentBattingTeamName = _gameState.IsTopInning ? "Away" : "Home";
        logs.Add($"\n--- Inning: {_gameState.CurrentInning} ({(_gameState.IsTopInning ? "Top" : "Bottom")}), Batting: {currentBattingTeamName} ---");

        while (_gameState.Outs < 3)
        {
            var (batter, pitcher) = GetCurrentMatchup();
            int basesOccupied = CountBasesOccupied();
            
            // 使用 PitcherManager 進行對決（會應用疲勞效果）
            var result = _simulator.SimulateAtBat(batter, pitcher, _pitcherManager);
            
            logs.Add($"Batter {batter.Name} vs Pitcher {pitcher.Name} (體力:{pitcher.CurrentStamina:F1}%, 投球數:{pitcher.PitchCount})... Result: {result.ResultType}");
            
            // 更新投手體力
            _pitcherManager.UpdateStamina(pitcher, result, basesOccupied);
            
            ProcessAtBatResult(result, logs);
            AdvanceBatterIndex();
            
            // 檢查是否需要緊急換投
            var decision = _pitcherManager.ShouldChangePitcher(pitcher, _gameState);
            if (decision.ShouldChange && decision.Priority <= 2)  // 高優先級立即換投
            {
                ChangePitcher(logs, decision);
            }
        }
    }
    
    private int CountBasesOccupied()
    {
        int count = 0;
        for (int i = 1; i <= 3; i++)
        {
            if (_gameState.Bases[i] != null) count++;
        }
        return count;
    }
    
    private void CheckAndChangePitcher(List<string> logs)
    {
        var currentPitcher = _gameState.IsTopInning ? _gameState.HomeTeamPitcher : _gameState.AwayTeamPitcher;
        var decision = _pitcherManager.ShouldChangePitcher(currentPitcher, _gameState);
        
        if (decision.ShouldChange)
        {
            ChangePitcher(logs, decision);
        }
    }
    
    private void ChangePitcher(List<string> logs, PitcherChangeDecision decision)
    {
        bool isHomeTeam = !_gameState.IsTopInning;
        var bullpen = isHomeTeam ? _gameState.HomeTeamBullpen : _gameState.AwayTeamBullpen;
        var oldPitcher = isHomeTeam ? _gameState.HomeTeamPitcher : _gameState.AwayTeamPitcher;
        
        var newPitcher = _pitcherManager.SelectReliever(bullpen, _gameState);
        
        if (newPitcher != null)
        {
            // 記錄換投
            var change = new PitcherChange
            {
                Inning = _gameState.CurrentInning,
                IsTopInning = _gameState.IsTopInning,
                OldPitcherName = oldPitcher.Name,
                NewPitcherName = newPitcher.Name,
                Reason = decision.Description
            };
            _gameState.PitcherChanges.Add(change);
            
            // 執行換投
            if (isHomeTeam)
                _gameState.HomeTeamPitcher = newPitcher;
            else
                _gameState.AwayTeamPitcher = newPitcher;
            
            // 初始化新投手
            _pitcherManager.InitializePitcher(newPitcher);
            
            // 從牛棚移除
            bullpen.Remove(newPitcher);
            
            // 記錄日誌
            logs.Add($"🔄 換投：{oldPitcher.Name} → {newPitcher.Name} ({decision.Description})");
        }
        else
        {
            logs.Add($"⚠️ 牛棚無可用投手，繼續使用 {oldPitcher.Name}");
        }
    }

    // --- Async Logic (for SignalR) ---
    private async Task RunHalfInningAsync(CancellationToken token)
    {
        // 半局開始前檢查是否需要換投
        await CheckAndChangePitcherAsync(token);
        
        _gameState.ResetForNewHalfInning();
        await SendGameUpdate($"Inning Start: {_gameState.CurrentInning} ({(_gameState.IsTopInning ? "Top" : "Bottom")})");

        while (_gameState.Outs < 3 && !token.IsCancellationRequested)
        {
            while (_isPaused) await Task.Delay(500, token);

            var (batter, pitcher) = GetCurrentMatchup();
            int basesOccupied = CountBasesOccupied();
            
            // 使用 PitcherManager 進行對決（會應用疲勞效果）
            var result = _simulator.SimulateAtBat(batter, pitcher, _pitcherManager);
            
            // 更新投手體力
            _pitcherManager.UpdateStamina(pitcher, result, basesOccupied);

            ProcessAtBatResult(result, null); // No logs needed for SignalR, just state update
            AdvanceBatterIndex();

            await SendGameUpdate($"Batter: {batter.Name} vs Pitcher: {pitcher.Name} (體力:{pitcher.CurrentStamina:F1}%, 投球數:{pitcher.PitchCount}), Result: {result.ResultType}");
            
            // 檢查是否需要緊急換投
            var decision = _pitcherManager.ShouldChangePitcher(pitcher, _gameState);
            if (decision.ShouldChange && decision.Priority <= 2)  // 高優先級立即換投
            {
                await ChangePitcherAsync(decision, token);
            }
            
            await Task.Delay(1000, token); // Delay for visual effect
        }
    }
    
    private async Task CheckAndChangePitcherAsync(CancellationToken token)
    {
        var currentPitcher = _gameState.IsTopInning ? _gameState.HomeTeamPitcher : _gameState.AwayTeamPitcher;
        var decision = _pitcherManager.ShouldChangePitcher(currentPitcher, _gameState);
        
        if (decision.ShouldChange)
        {
            await ChangePitcherAsync(decision, token);
        }
    }
    
    private async Task ChangePitcherAsync(PitcherChangeDecision decision, CancellationToken token)
    {
        bool isHomeTeam = !_gameState.IsTopInning;
        var bullpen = isHomeTeam ? _gameState.HomeTeamBullpen : _gameState.AwayTeamBullpen;
        var oldPitcher = isHomeTeam ? _gameState.HomeTeamPitcher : _gameState.AwayTeamPitcher;
        
        var newPitcher = _pitcherManager.SelectReliever(bullpen, _gameState);
        
        if (newPitcher != null)
        {
            // 記錄換投
            var change = new PitcherChange
            {
                Inning = _gameState.CurrentInning,
                IsTopInning = _gameState.IsTopInning,
                OldPitcherName = oldPitcher.Name,
                NewPitcherName = newPitcher.Name,
                Reason = decision.Description
            };
            _gameState.PitcherChanges.Add(change);
            
            // 執行換投
            if (isHomeTeam)
                _gameState.HomeTeamPitcher = newPitcher;
            else
                _gameState.AwayTeamPitcher = newPitcher;
            
            // 初始化新投手
            _pitcherManager.InitializePitcher(newPitcher);
            
            // 從牛棚移除
            bullpen.Remove(newPitcher);
            
            // 通知換投
            await SendGameUpdate($"🔄 換投：{oldPitcher.Name} → {newPitcher.Name} ({decision.Description})");
        }
        else
        {
            await SendGameUpdate($"⚠️ 牛棚無可用投手，繼續使用 {oldPitcher.Name}");
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

