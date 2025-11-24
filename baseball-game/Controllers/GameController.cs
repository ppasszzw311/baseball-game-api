using Microsoft.AspNetCore.Mvc;
using simulator_console.Models;
using simulator_console.Services.Simulator;

namespace simulator_console.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly GameSimulatorSerivce _gameService;

    public GameController(GameSimulatorSerivce gameService)
    {
        _gameService = gameService;
    }

    [HttpPost("simulate")]
    public ActionResult<GameResult> SimulateGame()
    {
        var result = _gameService.SimulateFullGame();
        return Ok(result);
    }
}
