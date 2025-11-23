using baseball_game.Services.PlayerPackage;

namespace baseball_game.Services.Simulator;

public class GameSimulatorSerivce
{
    private readonly PlayerSerivce _playerSerivce;

    public GameSimulatorSerivce(PlayerSerivce playerSerivce)
    {
        _playerSerivce = playerSerivce;
    }

    public void RunSimulation()
    {
        var pitcher = _playerSerivce.GetFatePlayer(PlayerType.pictor);
        var hitter = _playerSerivce.GetFatePlayer(PlayerType.hitter);
        
        // 
        Console.WriteLine($"打者： con: {hitter.Contract}, pow: {hitter.Power}, dis: {hitter.Discipline}");
        Console.WriteLine($"投手: str: {pitcher.Stamina}, con: {pitcher.Control}, mo: {pitcher.Movement}, ve: {pitcher.Velocity}");
        var sim = new Simulator();

        for (int i = 0; i < 10; i ++)
        {
            var reslut = sim.SimulateAtBat(hitter, pitcher);
            Console.WriteLine(reslut);
        }
    }
}