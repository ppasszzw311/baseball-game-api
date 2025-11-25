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
    
    /// <summary>
    /// 新增球隊
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TeamEntity>> CreateTeam([FromBody] CreateTeamRequest request)
    {
        // 檢查球隊數量上限
        var teamCount = await _context.Teams.CountAsync();
        if (teamCount >= 6)
        {
            return BadRequest(new { message = "已達到球隊數量上限（6隊）" });
        }
        
        var team = new TeamEntity
        {
            Name = request.Name,
            Abbreviation = request.Abbreviation,
            Color = request.Color
        };
        
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
    }
    
    /// <summary>
    /// 更新球隊資訊
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<TeamEntity>> UpdateTeam(int id, [FromBody] CreateTeamRequest request)
    {
        var team = await _context.Teams.FindAsync(id);
        if (team == null) return NotFound();
        
        team.Name = request.Name;
        team.Abbreviation = request.Abbreviation;
        team.Color = request.Color;
        
        await _context.SaveChangesAsync();
        return Ok(team);
    }
    
    /// <summary>
    /// 刪除球隊（會一併刪除該隊所有球員）
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTeam(int id)
    {
        var team = await _context.Teams.FindAsync(id);
        if (team == null) return NotFound();
        
        // 刪除該隊所有球員
        var players = await _context.Players.Where(p => p.TeamId == id).ToListAsync();
        _context.Players.RemoveRange(players);
        
        _context.Teams.Remove(team);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
    /// <summary>
    /// 新增球員到指定球隊
    /// </summary>
    [HttpPost("{teamId}/players")]
    public async Task<ActionResult<Player>> AddPlayer(int teamId, [FromBody] CreatePlayerRequest request)
    {
        // 檢查球隊是否存在
        var team = await _context.Teams.FindAsync(teamId);
        if (team == null) return NotFound(new { message = "球隊不存在" });
        
        // 檢查該隊球員數量
        var playerCount = await _context.Players.CountAsync(p => p.TeamId == teamId);
        if (playerCount >= 30)
        {
            return BadRequest(new { message = "該球隊已達到球員數量上限（30人）" });
        }
        
        var player = new Player
        {
            TeamId = teamId,
            Name = request.Name,
            Type = request.Type,
            Role = request.Role,
            // 打者屬性
            Contact = request.Contact,
            Power = request.Power,
            Vision = request.Vision,
            Speed = request.Speed,
            BaseRunning = request.BaseRunning,
            Fielding = request.Fielding,
            Arm = request.Arm,
            Position = request.Position,
            // 投手屬性
            Stamina = request.Stamina,
            Control = request.Control,
            Velocity = request.Velocity,
            Breaking = request.Breaking,
            Experience = request.Experience
        };
        
        _context.Players.Add(player);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetRoster), new { id = teamId }, player);
    }
    
    /// <summary>
    /// 更新球員資訊
    /// </summary>
    [HttpPut("{teamId}/players/{playerId}")]
    public async Task<ActionResult<Player>> UpdatePlayer(int teamId, int playerId, [FromBody] CreatePlayerRequest request)
    {
        var player = await _context.Players.FindAsync(playerId);
        if (player == null) return NotFound();
        if (player.TeamId != teamId) return BadRequest(new { message = "球員不屬於該球隊" });
        
        player.Name = request.Name;
        player.Type = request.Type;
        player.Role = request.Role;
        player.Contact = request.Contact;
        player.Power = request.Power;
        player.Vision = request.Vision;
        player.Speed = request.Speed;
        player.BaseRunning = request.BaseRunning;
        player.Fielding = request.Fielding;
        player.Arm = request.Arm;
        player.Position = request.Position;
        player.Stamina = request.Stamina;
        player.Control = request.Control;
        player.Velocity = request.Velocity;
        player.Breaking = request.Breaking;
        player.Experience = request.Experience;
        
        await _context.SaveChangesAsync();
        return Ok(player);
    }
    
    /// <summary>
    /// 刪除球員
    /// </summary>
    [HttpDelete("{teamId}/players/{playerId}")]
    public async Task<ActionResult> DeletePlayer(int teamId, int playerId)
    {
        var player = await _context.Players.FindAsync(playerId);
        if (player == null) return NotFound();
        if (player.TeamId != teamId) return BadRequest(new { message = "球員不屬於該球隊" });
        
        _context.Players.Remove(player);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}

// Request DTOs
public class CreateTeamRequest
{
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string Color { get; set; } = "#000000";
}

public class CreatePlayerRequest
{
    public string Name { get; set; } = string.Empty;
    public int Type { get; set; } // 0: Hitter, 1: Pitcher
    public int Role { get; set; } = 0; // 0: Starter, 1: MiddleReliever, 2: SetupMan, 3: Closer
    
    // 打者屬性
    public int Contact { get; set; } = 30;
    public int Power { get; set; } = 15;
    public int Vision { get; set; } = 20;
    public int Speed { get; set; } = 0;
    public int BaseRunning { get; set; } = 0;
    public int Fielding { get; set; } = 0;
    public int Arm { get; set; } = 0;
    public int Position { get; set; } = 0;
    
    // 投手屬性
    public int Stamina { get; set; } = 30;
    public int Control { get; set; } = 20;
    public int Velocity { get; set; } = 15;
    public int Breaking { get; set; } = 14;
    public int Experience { get; set; } = 0;
}
