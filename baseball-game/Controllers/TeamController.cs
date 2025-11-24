using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using simulator_console.Data;
using simulator_console.Models;

namespace simulator_console.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamController : ControllerBase
{
    private readonly GameDbContext _context;

    public TeamController(GameDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<TeamEntity>>> GetTeams()
    {
        return await _context.Teams.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TeamEntity>> GetTeam(int id)
    {
        var team = await _context.Teams.FindAsync(id);
        if (team == null) return NotFound();
        return Ok(team);
    }
    
    [HttpGet("{id}/roster")]
    public async Task<ActionResult<List<Player>>> GetRoster(int id)
    {
        var roster = await _context.Players
            .Where(p => p.TeamId == id)
            .ToListAsync();
        return Ok(roster);
    }
}
