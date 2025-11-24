using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using simulator_console.Data;
using simulator_console.Models;
using simulator_console.Services.Simulator;
using System.IO;

namespace simulator_console.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly GameSimulatorSerivce _gameService;
    private readonly GameDbContext _dbContext;

    public GameController(GameSimulatorSerivce gameService, GameDbContext dbContext)
    {
        _gameService = gameService;
        _dbContext = dbContext;
    }

    [HttpPost("simulate")]
    public ActionResult<GameResult> SimulateGame()
    {
        var result = _gameService.SimulateFullGame();
        return Ok(result);
    }

    [HttpPost("reset-db")]
    public IActionResult ResetDb()
    {
        try
        {
            // Ensure database is deleted to start fresh
            _dbContext.Database.EnsureDeleted();
            
            // Read SQL scripts
            var schemaSql = System.IO.File.ReadAllText("RebuildSqls/schema.sql");
            var seedSql = System.IO.File.ReadAllText("RebuildSqls/seed.sql");

            // Execute scripts
            _dbContext.Database.ExecuteSqlRaw(schemaSql);
            _dbContext.Database.ExecuteSqlRaw(seedSql);

            return Ok(new { message = "Database reset and seeded successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error resetting database: {ex.Message}" });
        }
    }
}
