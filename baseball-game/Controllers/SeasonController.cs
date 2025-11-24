using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using simulator_console.Data;
using simulator_console.Models;
using simulator_console.Services;

namespace simulator_console.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeasonController : ControllerBase
{
    private readonly SeasonService _seasonService;
    private readonly GameDbContext _context;

    public SeasonController(SeasonService seasonService, GameDbContext context)
    {
        _seasonService = seasonService;
        _context = context;
    }

    [HttpPost]
    public ActionResult<Season> StartSeason([FromBody] int year)
    {
        var season = _seasonService.StartNewSeason(year);
        return Ok(season);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Season>> GetSeason(string id)
    {
        var season = await _context.Seasons.FindAsync(id);
        if (season == null) return NotFound();
        return Ok(season);
    }

    [HttpGet("{id}/schedule")]
    public async Task<ActionResult<List<Schedule>>> GetSchedule(string id)
    {
        var schedule = await _context.Schedules
            .Where(s => s.SeasonId == id)
            .OrderBy(s => s.Day)
            .ToListAsync();
        return Ok(schedule);
    }
}
