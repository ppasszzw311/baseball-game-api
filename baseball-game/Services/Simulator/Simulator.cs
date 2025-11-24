using simulator_console.Models;

namespace simulator_console.Services.Simulator;

// 模擬投打對決的狀況
public class Simulator
{
    private readonly Random _random = new();
    public AtBatResult SimulateAtBat(Player batter, Player pitcher)
    {
        var atBatResult = new AtBatResult { Hitter = batter, Pitcher = pitcher };

        if (CheckWalk(batter, pitcher))
        {
            atBatResult.ResultType = AtBatType.Walk;
            return atBatResult;
        }
        
        // 攻擊方
        double attack =
            batter.Contract * 0.5 +
            batter.Power * 0.3 +
            batter.Discipline * 0.2;
        
        // 防禦方
        double defense =
            pitcher.Control * 0.3 +
            pitcher.Velocity * 0.4 +
            pitcher.Movement * 0.3;

        double result = attack - defense + RandomNoise();

        switch (result)
        {
            case < -20:
                atBatResult.ResultType = AtBatType.StrikeOut;
                break;
            case < -10:
                atBatResult.ResultType = AtBatType.Out;
                break;
            case < 10:
                atBatResult.ResultType = AtBatType.Single;
                break;
            case < 20:
                atBatResult.ResultType = AtBatType.Double;
                break;
            case < 30:
                atBatResult.ResultType = AtBatType.Triple;
                break;
            default:
                atBatResult.ResultType = AtBatType.HomeRun;
                break;
        }
        return atBatResult;
    }
    
    // 加上一些隨機性
    private double RandomNoise()
    {
        return (_random.NextDouble() * 20) - 10;
    }

    private double RandomNoise(int seed)
    {
        return _random.NextDouble() * seed;
    }
    
    
    // 判定會不會變成保送
    private bool CheckWalk(Player hitter, Player pitcher)
    {
        double walkFactor =
            hitter.Discipline * 0.6 - // 打者會不會選球
            pitcher.Control * 0.5 + // 投手控球好不好
            RandomNoise(8); // 變數
        
        return walkFactor > 20;
    }
}