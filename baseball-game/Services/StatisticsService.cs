using Microsoft.EntityFrameworkCore;
using simulator_console.Data;
using simulator_console.Models;

namespace simulator_console.Services;

/// <summary>
/// 統計服務 - 負責管理球員統計數據的計算與更新
/// </summary>
public class StatisticsService
{
    private readonly GameDbContext _context;

    public StatisticsService(GameDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 更新打者統計數據
    /// </summary>
    public async Task UpdateHittingStats(int playerId, string seasonId, AtBatResult result)
    {
        var stats = await GetOrCreateHittingStats(playerId, seasonId);
        
        stats.PlateAppearances++;
        
        switch (result.ResultType)
        {
            case AtBatType.Walk:
                stats.Walks++;
                break;
                
            case AtBatType.StrikeOut:
                stats.AtBats++;
                stats.Strikeouts++;
                break;
                
            case AtBatType.Out:
                stats.AtBats++;
                break;
                
            case AtBatType.Single:
                stats.AtBats++;
                stats.Hits++;
                stats.Singles++;
                break;
                
            case AtBatType.Double:
                stats.AtBats++;
                stats.Hits++;
                stats.Doubles++;
                break;
                
            case AtBatType.Triple:
                stats.AtBats++;
                stats.Hits++;
                stats.Triples++;
                break;
                
            case AtBatType.HomeRun:
                stats.AtBats++;
                stats.Hits++;
                stats.HomeRuns++;
                stats.RBI++; // 全壘打至少一分打點
                stats.Runs++; // 打者自己得分
                break;
        }
        
        stats.CalculateStats();
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 更新投手統計數據
    /// </summary>
    public async Task UpdatePitchingStats(int pitcherId, string seasonId, AtBatResult result, bool isEarnedRun = true)
    {
        var stats = await GetOrCreatePitchingStats(pitcherId, seasonId);
        
        stats.BattersFaced++;
        
        switch (result.ResultType)
        {
            case AtBatType.Walk:
                stats.WalksAllowed++;
                break;
                
            case AtBatType.StrikeOut:
                stats.Strikeouts++;
                // 三振計算為 1/3 局
                stats.InningsPitched += 1.0 / 3.0;
                break;
                
            case AtBatType.Out:
                // 出局計算為 1/3 局
                stats.InningsPitched += 1.0 / 3.0;
                break;
                
            case AtBatType.Single:
            case AtBatType.Double:
            case AtBatType.Triple:
                stats.HitsAllowed++;
                break;
                
            case AtBatType.HomeRun:
                stats.HitsAllowed++;
                stats.HomeRunsAllowed++;
                stats.RunsAllowed++;
                if (isEarnedRun)
                {
                    stats.EarnedRuns++;
                }
                break;
        }
        
        stats.CalculateStats();
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 記錄打者得分
    /// </summary>
    public async Task RecordRun(int playerId, string seasonId, int runsScored = 1)
    {
        var stats = await GetOrCreateHittingStats(playerId, seasonId);
        stats.Runs += runsScored;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 記錄打點
    /// </summary>
    public async Task RecordRBI(int playerId, string seasonId, int rbis = 1)
    {
        var stats = await GetOrCreateHittingStats(playerId, seasonId);
        stats.RBI += rbis;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 記錄投手失分
    /// </summary>
    public async Task RecordRunsAllowed(int pitcherId, string seasonId, int runs, bool isEarned = true)
    {
        var stats = await GetOrCreatePitchingStats(pitcherId, seasonId);
        stats.RunsAllowed += runs;
        if (isEarned)
        {
            stats.EarnedRuns += runs;
        }
        stats.CalculateStats();
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 記錄投手勝敗
    /// </summary>
    public async Task RecordPitcherDecision(int pitcherId, string seasonId, bool isWin)
    {
        var stats = await GetOrCreatePitchingStats(pitcherId, seasonId);
        if (isWin)
        {
            stats.Wins++;
        }
        else
        {
            stats.Losses++;
        }
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 獲取或建立打者統計
    /// </summary>
    private async Task<HittingStats> GetOrCreateHittingStats(int playerId, string seasonId)
    {
        var stats = await _context.HittingStats
            .FirstOrDefaultAsync(s => s.PlayerId == playerId && s.SeasonId == seasonId);
        
        if (stats == null)
        {
            stats = new HittingStats
            {
                PlayerId = playerId,
                SeasonId = seasonId
            };
            _context.HittingStats.Add(stats);
            await _context.SaveChangesAsync();
        }
        
        return stats;
    }

    /// <summary>
    /// 獲取或建立投手統計
    /// </summary>
    private async Task<PitchingStats> GetOrCreatePitchingStats(int pitcherId, string seasonId)
    {
        var stats = await _context.PitchingStats
            .FirstOrDefaultAsync(s => s.PlayerId == pitcherId && s.SeasonId == seasonId);
        
        if (stats == null)
        {
            stats = new PitchingStats
            {
                PlayerId = pitcherId,
                SeasonId = seasonId
            };
            _context.PitchingStats.Add(stats);
            await _context.SaveChangesAsync();
        }
        
        return stats;
    }

    /// <summary>
    /// 獲取賽季打者排行榜
    /// </summary>
    public async Task<List<HittingStats>> GetHittingLeaders(string seasonId, int topN = 10)
    {
        return await _context.HittingStats
            .Include(s => s.Player)
            .Where(s => s.SeasonId == seasonId && s.AtBats >= 50) // 至少50打數才列入排行
            .OrderByDescending(s => s.BattingAverage)
            .Take(topN)
            .ToListAsync();
    }

    /// <summary>
    /// 獲取賽季投手排行榜
    /// </summary>
    public async Task<List<PitchingStats>> GetPitchingLeaders(string seasonId, int topN = 10)
    {
        return await _context.PitchingStats
            .Include(s => s.Player)
            .Where(s => s.SeasonId == seasonId && s.InningsPitched >= 10) // 至少投10局才列入排行
            .OrderBy(s => s.ERA)
            .Take(topN)
            .ToListAsync();
    }

    /// <summary>
    /// 獲取球員賽季統計
    /// </summary>
    public async Task<HittingStats?> GetPlayerHittingStats(int playerId, string seasonId)
    {
        return await _context.HittingStats
            .Include(s => s.Player)
            .FirstOrDefaultAsync(s => s.PlayerId == playerId && s.SeasonId == seasonId);
    }

    /// <summary>
    /// 獲取投手賽季統計
    /// </summary>
    public async Task<PitchingStats?> GetPlayerPitchingStats(int playerId, string seasonId)
    {
        return await _context.PitchingStats
            .Include(s => s.Player)
            .FirstOrDefaultAsync(s => s.PlayerId == playerId && s.SeasonId == seasonId);
    }
    
    /// <summary>
    /// 批量更新打者統計（用於比賽結束後）
    /// </summary>
    public async Task BatchUpdateHittingStats(int playerId, string seasonId, GameHittingStats gameStats)
    {
        var stats = await GetOrCreateHittingStats(playerId, seasonId);
        
        stats.PlateAppearances += gameStats.PlateAppearances;
        stats.AtBats += gameStats.AtBats;
        stats.Hits += gameStats.Hits;
        stats.Singles += gameStats.Singles;
        stats.Doubles += gameStats.Doubles;
        stats.Triples += gameStats.Triples;
        stats.HomeRuns += gameStats.HomeRuns;
        stats.Runs += gameStats.Runs;
        stats.RBI += gameStats.RBI;
        stats.Walks += gameStats.Walks;
        stats.Strikeouts += gameStats.Strikeouts;
        
        stats.CalculateStats();
        await _context.SaveChangesAsync();
    }
    
    /// <summary>
    /// 批量更新投手統計（用於比賽結束後）
    /// </summary>
    public async Task BatchUpdatePitchingStats(int playerId, string seasonId, GamePitchingStats gameStats)
    {
        var stats = await GetOrCreatePitchingStats(playerId, seasonId);
        
        stats.BattersFaced += gameStats.BattersFaced;
        stats.HitsAllowed += gameStats.HitsAllowed;
        stats.RunsAllowed += gameStats.RunsAllowed;
        stats.EarnedRuns += gameStats.EarnedRuns;
        stats.HomeRunsAllowed += gameStats.HomeRunsAllowed;
        stats.WalksAllowed += gameStats.WalksAllowed;
        stats.Strikeouts += gameStats.Strikeouts;
        
        // 計算投球局數（使用出局數除以3）
        stats.InningsPitched += gameStats.OutsRecorded / 3.0;
        
        // 更新出賽數（每場比賽至少算一次）
        stats.GamesPlayed++;
        
        stats.CalculateStats();
        await _context.SaveChangesAsync();
    }
}

// 比賽統計追蹤類別
public class GameHittingStats
{
    public int PlayerId { get; set; }
    public int PlateAppearances { get; set; }
    public int AtBats { get; set; }
    public int Hits { get; set; }
    public int Singles { get; set; }
    public int Doubles { get; set; }
    public int Triples { get; set; }
    public int HomeRuns { get; set; }
    public int Runs { get; set; }
    public int RBI { get; set; }
    public int Walks { get; set; }
    public int Strikeouts { get; set; }
}

public class GamePitchingStats
{
    public int PlayerId { get; set; }
    public int BattersFaced { get; set; }
    public int HitsAllowed { get; set; }
    public int RunsAllowed { get; set; }
    public int EarnedRuns { get; set; }
    public int HomeRunsAllowed { get; set; }
    public int WalksAllowed { get; set; }
    public int Strikeouts { get; set; }
    public int OutsRecorded { get; set; }  // 用於計算局數
}
