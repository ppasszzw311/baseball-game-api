using simulator_console.Models;

namespace simulator_console.Services.Simulator;

// 模擬投打對決的狀況
public class Simulator
{
    private readonly Random _random = new();
    
    public AtBatResult SimulateAtBat(Player batter, Player pitcher, PitcherManager? pitcherManager = null)
    {
        // 如果提供了 PitcherManager，應用疲勞效果
        var effectivePitcher = pitcherManager != null 
            ? pitcherManager.ApplyFatigueEffect(pitcher) 
            : pitcher;
        
        var atBatResult = new AtBatResult { Hitter = batter, Pitcher = pitcher };
        var pitchCount = new PitchCount();
        
        // 逐球模擬投打對決
        while (pitchCount.Strikes < 3 && pitchCount.Balls < 4)
        {
            bool isStrike = SimulatePitch(batter, effectivePitcher);
            
            if (isStrike)
            {
                pitchCount.Strikes++;
                
                // 第三好球 - 可能三振或擊出
                if (pitchCount.Strikes == 3)
                {
                    // 判斷是否能打到球
                    if (CanMakeContact(batter, effectivePitcher))
                    {
                        // 打到球，判定結果
                        atBatResult = DetermineHitResult(batter, effectivePitcher);
                    }
                    else
                    {
                        // 沒打到 - 三振
                        atBatResult.ResultType = AtBatType.StrikeOut;
                    }
                    break;
                }
            }
            else
            {
                pitchCount.Balls++;
                
                // 第四壞球 - 保送
                if (pitchCount.Balls == 4)
                {
                    atBatResult.ResultType = AtBatType.Walk;
                    break;
                }
            }
            
            // 兩好球前，有機會提早擊出
            if (pitchCount.Strikes <= 2 && _random.NextDouble() < GetSwingProbability(batter, pitchCount))
            {
                if (CanMakeContact(batter, effectivePitcher))
                {
                    atBatResult = DetermineHitResult(batter, effectivePitcher);
                    break;
                }
                // 揮空算好球
                pitchCount.Strikes++;
            }
        }
        
        atBatResult.PitchCount = pitchCount;
        return atBatResult;
    }
    
    /// <summary>
    /// 模擬單一投球是否為好球
    /// </summary>
    private bool SimulatePitch(Player batter, Player pitcher)
    {
        // 投手控球能力 vs 打者選球能力
        double strikeZoneControl = pitcher.Control * 0.7 + pitcher.Velocity * 0.2;
        double plateVision = batter.Vision * 0.5;
        
        double strikeProbability = (strikeZoneControl - plateVision * 0.3) / 150.0;
        strikeProbability = Math.Clamp(strikeProbability, 0.35, 0.65); // 35%-65%好球率，平衡三振和保送
        
        return _random.NextDouble() < strikeProbability;
    }
    
    /// <summary>
    /// 判斷打者是否能碰到球
    /// </summary>
    private bool CanMakeContact(Player batter, Player pitcher)
    {
        double contactAbility = batter.Contact * 0.7 + batter.Vision * 0.3;
        double pitchDifficulty = pitcher.Velocity * 0.4 + pitcher.Breaking * 0.4 + pitcher.Control * 0.2;
        
        double contactProbability = (contactAbility - pitchDifficulty * 0.4) / 100.0;
        contactProbability = Math.Clamp(contactProbability, 0.55, 0.90); // 55%-90%接觸率，確保更多擊球出去
        
        return _random.NextDouble() < contactProbability;
    }
    
    /// <summary>
    /// 決定打擊結果（僅判定擊球品質，守備結果由 DefenseManager 處理）
    /// </summary>
    private AtBatResult DetermineHitResult(Player batter, Player pitcher)
    {
        var result = new AtBatResult { Hitter = batter, Pitcher = pitcher };
        
        // 攻擊方能力
        double attack = batter.Contact * 0.4 + batter.Power * 0.5 + batter.Vision * 0.1;
        
        // 防禦方能力
        double defense = pitcher.Control * 0.3 + pitcher.Velocity * 0.35 + pitcher.Breaking * 0.35;
        
        double score = attack - defense + RandomNoise() * 3;
        
        // 判定擊球品質（守備系統將根據此品質決定最終結果）
        result.Quality = score switch
        {
            < -20 => BallQuality.WeakGroundBall,  // 軟弱滾地球
            < -5 => BallQuality.GroundBall,       // 滾地球
            < 10 => BallQuality.LineDrive,        // 平飛球
            < 20 => BallQuality.FlyBall,          // 飛球
            < 30 => BallQuality.DeepFlyBall,      // 深遠飛球
            _ => BallQuality.Bomb                  // 強力長打
        };
        
        // 暫時設為 Out，實際結果由守備系統決定
        result.ResultType = AtBatType.Out;
        
        return result;
    }
    
    /// <summary>
    /// 計算打者揮棒機率（根據球數）
    /// </summary>
    private double GetSwingProbability(Player batter, PitchCount count)
    {
        // 提高基礎揮棒率，讓更多球被擊出
        double baseSwingRate = 0.5;
        
        // 根據球數調整
        if (count.Balls > count.Strikes)
        {
            // 打者有利，更有耐心
            baseSwingRate -= 0.1;
        }
        else if (count.Strikes > count.Balls)
        {
            // 投手有利，打者較積極
            baseSwingRate += 0.2;
        }
        
        // 根據打者選球能力調整
        baseSwingRate -= (batter.Vision - 50) / 500.0;
        
        return Math.Clamp(baseSwingRate, 0.3, 0.7);
    }
    
    // 加上一些隨機性
    private double RandomNoise()
    {
        return (_random.NextDouble() * 20) - 10;
    }
}