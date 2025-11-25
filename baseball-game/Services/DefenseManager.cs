using simulator_console.Models;

namespace simulator_console.Services;

/// <summary>
/// 守備位置枚舉
/// </summary>
public enum FieldPosition
{
    Pitcher = 1,      // 投手
    Catcher = 2,      // 捕手
    FirstBase = 3,    // 一壘手
    SecondBase = 4,   // 二壘手
    ThirdBase = 5,    // 三壘手
    ShortStop = 6,    // 遊擊手
    LeftField = 7,    // 左外野手
    CenterField = 8,  // 中外野手
    RightField = 9    // 右外野手
}

/// <summary>
/// 守備結果
/// </summary>
public class DefenseResult
{
    public bool IsOut { get; set; }              // 是否出局
    public bool IsError { get; set; }            // 是否失誤
    public AtBatType FinalResult { get; set; }   // 最終結果
    public FieldPosition? FielderPosition { get; set; }  // 守備者位置
    public Player? Fielder { get; set; }         // 守備者
    public string Description { get; set; } = string.Empty;  // 描述
}

/// <summary>
/// 守備管理器 - 處理守備判定邏輯
/// </summary>
public class DefenseManager
{
    private readonly Random _random = new();

    /// <summary>
    /// 根據擊球品質和守備陣容判定結果
    /// </summary>
    public DefenseResult ProcessDefense(BallQuality ballQuality, Player batter, List<Player> defenders, Player pitcher)
    {
        var result = new DefenseResult();

        // 根據擊球品質決定主要守備者和難度
        var (position, difficulty) = GetFielderAndDifficulty(ballQuality, batter);
        result.FielderPosition = position;

        // 找到對應位置的守備員（如果有的話）
        var fielder = GetFielderByPosition(defenders, position);
        result.Fielder = fielder;

        // 如果是投手位置，使用投手
        if (position == FieldPosition.Pitcher)
        {
            fielder = pitcher;
            result.Fielder = pitcher;
        }

        // 判定守備是否成功
        bool defenseSuccess = JudgeDefense(fielder, difficulty, ballQuality);
        result.IsError = !defenseSuccess;

        // 根據守備結果決定最終打擊結果
        result.FinalResult = DetermineFinalResult(ballQuality, defenseSuccess, batter);
        result.IsOut = result.FinalResult == AtBatType.Out;
        result.Description = GenerateDescription(ballQuality, position, defenseSuccess, result.FinalResult);

        return result;
    }

    /// <summary>
    /// 根據擊球品質決定守備位置和難度
    /// </summary>
    private (FieldPosition position, double difficulty) GetFielderAndDifficulty(BallQuality quality, Player batter)
    {
        // 根據擊球品質和隨機性決定球的方向
        double rand = _random.NextDouble();

        return quality switch
        {
            BallQuality.WeakGroundBall => 
                rand < 0.4 ? (FieldPosition.Pitcher, 0.2) :
                rand < 0.7 ? (FieldPosition.FirstBase, 0.3) :
                (FieldPosition.SecondBase, 0.3),

            BallQuality.GroundBall =>
                rand < 0.25 ? (FieldPosition.ThirdBase, 0.5) :
                rand < 0.5 ? (FieldPosition.ShortStop, 0.5) :
                rand < 0.75 ? (FieldPosition.SecondBase, 0.5) :
                (FieldPosition.FirstBase, 0.5),

            BallQuality.LineDrive =>
                rand < 0.15 ? (FieldPosition.Pitcher, 0.85) :
                rand < 0.3 ? (FieldPosition.FirstBase, 0.7) :
                rand < 0.5 ? (FieldPosition.SecondBase, 0.7) :
                rand < 0.65 ? (FieldPosition.ShortStop, 0.7) :
                rand < 0.8 ? (FieldPosition.ThirdBase, 0.7) :
                (rand < 0.9 ? (FieldPosition.LeftField, 0.6) : (FieldPosition.RightField, 0.6)),

            BallQuality.FlyBall =>
                rand < 0.33 ? (FieldPosition.LeftField, 0.4) :
                rand < 0.66 ? (FieldPosition.CenterField, 0.4) :
                (FieldPosition.RightField, 0.4),

            BallQuality.DeepFlyBall =>
                rand < 0.33 ? (FieldPosition.LeftField, 0.7) :
                rand < 0.66 ? (FieldPosition.CenterField, 0.7) :
                (FieldPosition.RightField, 0.7),

            BallQuality.Bomb =>
                // 強力擊球幾乎無法守備
                rand < 0.5 ? (FieldPosition.CenterField, 0.95) :
                (FieldPosition.LeftField, 0.95),

            _ => (FieldPosition.Pitcher, 0.5)
        };
    }

    /// <summary>
    /// 根據位置找到守備員
    /// </summary>
    private Player? GetFielderByPosition(List<Player> defenders, FieldPosition position)
    {
        return defenders.FirstOrDefault(p => p.Position == (int)position);
    }

    /// <summary>
    /// 判定守備是否成功
    /// </summary>
    private bool JudgeDefense(Player? fielder, double difficulty, BallQuality ballQuality)
    {
        if (fielder == null)
        {
            // 沒有守備員，視為失誤
            return false;
        }

        // 計算守備成功率
        double fieldingAbility = fielder.Fielding * 0.7 + fielder.Arm * 0.2 + fielder.Speed * 0.1;
        double successRate = (fieldingAbility / 100.0) * (1.0 - difficulty);

        // 特殊處理：Bomb 幾乎不可能守備成功
        if (ballQuality == BallQuality.Bomb)
        {
            successRate *= 0.1;
        }

        // 確保成功率在合理範圍
        successRate = Math.Clamp(successRate, 0.05, 0.98);

        return _random.NextDouble() < successRate;
    }

    /// <summary>
    /// 根據守備結果決定最終打擊結果
    /// </summary>
    private AtBatType DetermineFinalResult(BallQuality quality, bool defenseSuccess, Player batter)
    {
        if (!defenseSuccess)
        {
            // 守備失誤，打者至少上壘
            return quality switch
            {
                BallQuality.WeakGroundBall => AtBatType.Single,
                BallQuality.GroundBall => AtBatType.Single,
                BallQuality.LineDrive => _random.NextDouble() < 0.7 ? AtBatType.Single : AtBatType.Double,
                BallQuality.FlyBall => _random.NextDouble() < 0.5 ? AtBatType.Single : AtBatType.Double,
                BallQuality.DeepFlyBall => _random.NextDouble() < 0.3 ? AtBatType.Double : AtBatType.Triple,
                BallQuality.Bomb => DetermineExtraBaseHit(batter.Power, true),
                _ => AtBatType.Single
            };
        }

        // 守備成功
        return quality switch
        {
            BallQuality.WeakGroundBall => AtBatType.Out,
            BallQuality.GroundBall => AtBatType.Out,
            BallQuality.LineDrive => _random.NextDouble() < 0.6 ? AtBatType.Out : AtBatType.Single,
            BallQuality.FlyBall => AtBatType.Out,
            BallQuality.DeepFlyBall => _random.NextDouble() < 0.2 ? AtBatType.Out : DetermineExtraBaseHit(batter.Power, false),
            BallQuality.Bomb => DetermineExtraBaseHit(batter.Power, true),
            _ => AtBatType.Out
        };
    }

    /// <summary>
    /// 判定長打類型（從 Simulator 複製）
    /// </summary>
    private AtBatType DetermineExtraBaseHit(int power, bool isBomb)
    {
        double rand = _random.NextDouble();
        double powerFactor = power / 100.0;

        if (isBomb)
        {
            double homeRunThreshold = 0.5 + powerFactor * 0.3;
            if (rand < homeRunThreshold) return AtBatType.HomeRun;
            if (rand < 0.8) return AtBatType.Triple;
            if (rand < 0.95) return AtBatType.Double;
            return AtBatType.Single;
        }
        else
        {
            double homeRunThreshold = 0.2 + powerFactor * 0.2;
            if (rand < homeRunThreshold) return AtBatType.HomeRun;
            if (rand < 0.4) return AtBatType.Triple;
            if (rand < 0.7) return AtBatType.Double;
            return AtBatType.Single;
        }
    }

    /// <summary>
    /// 生成守備描述
    /// </summary>
    private string GenerateDescription(BallQuality quality, FieldPosition position, bool success, AtBatType result)
    {
        string positionName = position switch
        {
            FieldPosition.Pitcher => "投手",
            FieldPosition.Catcher => "捕手",
            FieldPosition.FirstBase => "一壘手",
            FieldPosition.SecondBase => "二壘手",
            FieldPosition.ThirdBase => "三壘手",
            FieldPosition.ShortStop => "遊擊手",
            FieldPosition.LeftField => "左外野手",
            FieldPosition.CenterField => "中外野手",
            FieldPosition.RightField => "右外野手",
            _ => "守備員"
        };

        string qualityDesc = quality switch
        {
            BallQuality.WeakGroundBall => "軟弱的滾地球",
            BallQuality.GroundBall => "滾地球",
            BallQuality.LineDrive => "強勁的平飛球",
            BallQuality.FlyBall => "飛球",
            BallQuality.DeepFlyBall => "深遠的飛球",
            BallQuality.Bomb => "強力的長打",
            _ => "擊球"
        };

        if (!success)
        {
            return $"{qualityDesc}飛向{positionName}，守備失誤！";
        }

        if (result == AtBatType.Out)
        {
            return $"{qualityDesc}被{positionName}接殺出局！";
        }

        return $"{qualityDesc}突破{positionName}的防守形成安打！";
    }
}
