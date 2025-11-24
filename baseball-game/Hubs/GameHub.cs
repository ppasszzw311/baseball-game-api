using Microsoft.AspNetCore.SignalR;
using simulator_console.Services.Simulator;

namespace simulator_console.Hubs;

public class GameHub : Hub
{
    private readonly GameSimulatorSerivce _gameService;

    public GameHub(GameSimulatorSerivce gameService)
    {
        _gameService = gameService;
    }

    public async Task StartGame()
    {
        await _gameService.StartInteractiveGame();
    }

    public void PauseGame()
    {
        _gameService.PauseGame();
    }

    public void ResumeGame()
    {
        _gameService.ResumeGame();
    }

    public async Task RestartGame()
    {
        await _gameService.StartInteractiveGame();
    }
}
