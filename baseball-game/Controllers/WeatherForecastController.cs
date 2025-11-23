using baseball_game.Services.Simulator;
using Microsoft.AspNetCore.Mvc;

namespace baseball_game.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly GameSimulatorSerivce _gameSimulatorSerivce;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, GameSimulatorSerivce gameSimulatorSerivce)
    {
        _logger = logger;
        _gameSimulatorSerivce = gameSimulatorSerivce;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }

    [HttpGet("simulate", Name = "GetSimulate")]
    public ActionResult GetSimulate()
    {
        _gameSimulatorSerivce.RunSimulation();
        return Ok();
    }
}