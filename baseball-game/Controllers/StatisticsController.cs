using Microsoft.AspNetCore.Mvc;
using simulator_console.Models;
using simulator_console.Services;

namespace simulator_console.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly StatisticsService _statsService;

    public StatisticsController(StatisticsService statsService)
    {
        _statsService = statsService;
    }

    /// <summary>
    /// 獲取賽季打擊排行榜
    /// </summary>
    [HttpGet("hitting/leaders/{seasonId}")]
    public async Task<ActionResult<List<HittingStats>>> GetHittingLeaders(string seasonId, [FromQuery] int top = 10)
    {
        var leaders = await _statsService.GetHittingLeaders(seasonId, top);
        return Ok(leaders);
    }

    /// <summary>
    /// 獲取賽季投手排行榜
    /// </summary>
    [HttpGet("pitching/leaders/{seasonId}")]
    public async Task<ActionResult<List<PitchingStats>>> GetPitchingLeaders(string seasonId, [FromQuery] int top = 10)
    {
        var leaders = await _statsService.GetPitchingLeaders(seasonId, top);
        return Ok(leaders);
    }

    /// <summary>
    /// 獲取球員打擊統計
    /// </summary>
    [HttpGet("hitting/player/{playerId}/season/{seasonId}")]
    public async Task<ActionResult<HittingStats>> GetPlayerHittingStats(int playerId, string seasonId)
    {
        var stats = await _statsService.GetPlayerHittingStats(playerId, seasonId);
        if (stats == null)
        {
            return NotFound(new { message = "Statistics not found for this player and season" });
        }
        return Ok(stats);
    }

    /// <summary>
    /// 獲取投手統計
    /// </summary>
    [HttpGet("pitching/player/{playerId}/season/{seasonId}")]
    public async Task<ActionResult<PitchingStats>> GetPlayerPitchingStats(int playerId, string seasonId)
    {
        var stats = await _statsService.GetPlayerPitchingStats(playerId, seasonId);
        if (stats == null)
        {
            return NotFound(new { message = "Statistics not found for this pitcher and season" });
        }
        return Ok(stats);
    }
}
