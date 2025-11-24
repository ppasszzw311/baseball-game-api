# 投手疲勞與自動換投系統規格文件

📅 建立日期：2025-11-24  
📌 狀態：實作中  
🎯 目標：實現真實的投手疲勞機制與智能自動換投系統

---

## 📋 目錄

1. [系統概述](#系統概述)
2. [資料模型擴充](#資料模型擴充)
3. [疲勞計算機制](#疲勞計算機制)
4. [自動換投判定](#自動換投判定)
5. [牛棚投手選擇](#牛棚投手選擇)
6. [實作架構](#實作架構)
7. [資料庫調整](#資料庫調整)
8. [開發步驟](#開發步驟)
9. [測試情境](#測試情境)
10. [未來擴充](#未來擴充)

---

## 系統概述

### 功能目標

實現投手在比賽中的真實疲勞累積，當投手疲勞、失分過多或達到特定條件時，系統自動選擇適當的後援投手進行換投。

### 核心特性

- ✅ 投球數追蹤與體力消耗
- ✅ 體力影響投手能力值
- ✅ 多條件自動換投判定
- ✅ 智能後援投手選擇
- ✅ 投手角色分類系統
- ✅ 緊急換投機制

---

## 資料模型擴充

### 1. Player 模型新增屬性

```csharp
namespace simulator_console.Models;

public class Player
{
    // ... 現有屬性 ...
    
    // 投手角色分類
    public int Role { get; set; } = 0;  // 0:先發 1:中繼 2:佈局 3:終結者
    
    // 投手即時狀態（不存資料庫，僅在比賽中使用）
    [NotMapped]
    public double CurrentStamina { get; set; } = 100.0;  // 當前體力 (0-100)
    
    [NotMapped]
    public int PitchCount { get; set; } = 0;  // 本場投球數
    
    [NotMapped]
    public int ConsecutiveHitsAllowed { get; set; } = 0;  // 連續被安打數
}
```

### 2. GameState 模型擴充

```csharp
namespace simulator_console.Models;

public class GameState
{
    // ... 現有屬性 ...
    
    // 投手狀態追蹤
    public int HomeTeamPitcherPitchCount { get; set; } = 0;
    public int AwayTeamPitcherPitchCount { get; set; } = 0;
    public double HomeTeamPitcherStamina { get; set; } = 100.0;
    public double AwayTeamPitcherStamina { get; set; } = 100.0;
    
    // 牛棚管理
    public List<Player> HomeTeamBullpen { get; set; } = new();
    public List<Player> AwayTeamBullpen { get; set; } = new();
    
    // 本局失分追蹤
    public int HomeTeamRunsAllowedThisInning { get; set; } = 0;
    public int AwayTeamRunsAllowedThisInning { get; set; } = 0;
    
    // 累計失分
    public int HomeTeamRunsAllowed { get; set; } = 0;
    public int AwayTeamRunsAllowed { get; set; } = 0;
    
    // 換投歷史
    public List<PitcherChange> PitcherChanges { get; set; } = new();
}

public class PitcherChange
{
    public int Inning { get; set; }
    public bool IsTopInning { get; set; }
    public string OldPitcherName { get; set; } = string.Empty;
    public string NewPitcherName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
```

### 3. 投手角色枚舉

```csharp
namespace simulator_console.Models;

public enum PitcherRole
{
    Starter = 0,          // 先發投手（1-5號）
    MiddleReliever = 1,   // 中繼投手（6-7局）
    SetupMan = 2,         // 佈局投手（8局）
    Closer = 3            // 終結者（9局救援）
}

public enum PitcherChangeReason
{
    Fatigue,              // 疲勞
    PitchCount,           // 投球數過多
    RunsAllowed,          // 失分過多
    InningLimit,          // 局數限制
    Emergency,            // 緊急狀況（連續被安打）
    Strategic             // 戰術換投
}
```

---

## 疲勞計算機制

### 1. 基礎體力消耗公式

```csharp
// 每投一球的基本消耗
double baseStaminaCost = 100.0 / (pitcher.Stamina * 0.8);

// 範例：
// Stamina = 80: 每球消耗 ≈ 1.56 體力 (可投約 64 球)
// Stamina = 60: 每球消耗 ≈ 2.08 體力 (可投約 48 球)
// Stamina = 40: 每球消耗 ≈ 3.13 體力 (可投約 32 球)
```

### 2. 比賽事件影響體力

| 事件類型 | 體力消耗倍率 | 原因說明 |
|---------|-------------|---------|
| **三振** | 1.2x | 需要更用力投球，球數較多 |
| **保送** | 1.5x | 球數多，心理壓力大 |
| **被安打** | 1.3x | 心理壓力，投球失誤 |
| **被全壘打** | 2.0x | 重大打擊，士氣影響 |
| **一般出局** | 1.0x | 正常消耗 |

**額外消耗：**
```csharp
// 壘上有人時額外消耗
if (BasesOccupied > 0)
{
    staminaCost += 2.0 * BasesOccupied;  // 每個壘包 +2 體力
}
```

### 3. 體力影響能力值

```csharp
public void ApplyFatigueEffect(Player pitcher)
{
    if (pitcher.CurrentStamina >= 70)
    {
        // 體力充沛，無影響
        return;
    }
    else if (pitcher.CurrentStamina >= 50)
    {
        // 輕度疲勞 (50-69%)
        double factor = 0.85 + (pitcher.CurrentStamina - 50) / 100.0;
        pitcher.Control = (int)(pitcher.Control * factor);
        pitcher.Breaking = (int)(pitcher.Breaking * factor);
    }
    else if (pitcher.CurrentStamina >= 30)
    {
        // 中度疲勞 (30-49%)
        double factor = 0.7 + (pitcher.CurrentStamina - 30) / 100.0;
        pitcher.Control = (int)(pitcher.Control * factor);
        pitcher.Velocity = (int)(pitcher.Velocity * 0.95);
        pitcher.Breaking = (int)(pitcher.Breaking * factor);
    }
    else
    {
        // 嚴重疲勞 (<30%)
        double factor = 0.5 + pitcher.CurrentStamina / 100.0;
        pitcher.Control = (int)(pitcher.Control * factor);
        pitcher.Velocity = (int)(pitcher.Velocity * 0.85);
        pitcher.Breaking = (int)(pitcher.Breaking * factor);
    }
}
```

### 4. 疲勞等級定義

| 體力範圍 | 疲勞等級 | 能力影響 | 狀態描述 |
|---------|---------|---------|---------|
| 90-100% | 優秀 | 無影響 | 精神飽滿 |
| 70-89% | 良好 | 無影響 | 狀態良好 |
| 50-69% | 輕度疲勞 | -15% Control/Breaking | 開始疲累 |
| 30-49% | 中度疲勞 | -30% Control/Breaking, -5% Velocity | 明顯下降 |
| 10-29% | 嚴重疲勞 | -50% Control/Breaking, -15% Velocity | 急需休息 |
| 0-9% | 力竭 | -70% 所有能力 | 無法投球 |

---

## 自動換投判定

### 1. 換投觸發條件

系統在以下時機檢查是否需要換投：
- ✅ 每個半局結束後
- ✅ 單局失分達到閾值時
- ✅ 投手體力過低時
- ✅ 連續被安打時

### 2. 換投條件表

| 條件類型 | 判定閾值 | 優先級 | 說明 |
|---------|---------|--------|------|
| **體力耗盡** | CurrentStamina < 20 | 🔴 極高 | 強制換投 |
| **投球數限制** | PitchCount > 100 | 🔴 極高 | 超過負荷 |
| **單局失分** | 本局失分 ≥ 3 | 🟡 高 | 狀態不佳 |
| **累計失分** | 總失分 ≥ 5 | 🟡 高 | 被打爆 |
| **連續被安打** | 連續 3 支安打 | 🟡 高 | 緊急換投 |
| **局數限制** | 投滿 7 局 | 🟢 中 | 體力考量 |
| **體力偏低** | CurrentStamina < 40 | 🟢 中 | 預防性換投 |

### 3. 判定邏輯代碼

```csharp
public class PitcherChangeDecision
{
    public bool ShouldChange { get; set; }
    public PitcherChangeReason Reason { get; set; }
    public int Priority { get; set; }  // 1=最高, 3=最低
}

public PitcherChangeDecision ShouldChangePitcher(Player pitcher, GameState state)
{
    // 極高優先級條件
    if (pitcher.CurrentStamina < 20)
        return new PitcherChangeDecision 
        { 
            ShouldChange = true, 
            Reason = PitcherChangeReason.Fatigue, 
            Priority = 1 
        };
    
    if (pitcher.PitchCount > 100)
        return new PitcherChangeDecision 
        { 
            ShouldChange = true, 
            Reason = PitcherChangeReason.PitchCount, 
            Priority = 1 
        };
    
    // 高優先級條件
    int runsThisInning = state.IsTopInning 
        ? state.HomeTeamRunsAllowedThisInning 
        : state.AwayTeamRunsAllowedThisInning;
    
    if (runsThisInning >= 3)
        return new PitcherChangeDecision 
        { 
            ShouldChange = true, 
            Reason = PitcherChangeReason.RunsAllowed, 
            Priority = 2 
        };
    
    if (pitcher.ConsecutiveHitsAllowed >= 3)
        return new PitcherChangeDecision 
        { 
            ShouldChange = true, 
            Reason = PitcherChangeReason.Emergency, 
            Priority = 2 
        };
    
    // 中優先級條件
    if (state.CurrentInning >= 7 && pitcher.Role == (int)PitcherRole.Starter)
        return new PitcherChangeDecision 
        { 
            ShouldChange = true, 
            Reason = PitcherChangeReason.InningLimit, 
            Priority = 3 
        };
    
    if (pitcher.CurrentStamina < 40)
        return new PitcherChangeDecision 
        { 
            ShouldChange = true, 
            Reason = PitcherChangeReason.Fatigue, 
            Priority = 3 
        };
    
    return new PitcherChangeDecision { ShouldChange = false };
}
```

---

## 牛棚投手選擇

### 1. 投手角色配置建議

| 角色 | 數量 | 使用時機 | 特性要求 |
|------|------|---------|---------|
| 先發投手 (Starter) | 1 | 1-7局 | 高 Stamina |
| 中繼投手 (Middle Reliever) | 2-3 | 6-7局 | 均衡能力 |
| 佈局投手 (Setup Man) | 1 | 8局 | 高 Control |
| 終結者 (Closer) | 1 | 9局領先時 | 高 Velocity + Control |

### 2. 後援投手選擇邏輯

```csharp
public Player? SelectReliever(List<Player> bullpen, GameState state)
{
    var availablePitchers = bullpen
        .Where(p => p.CurrentStamina >= 80)  // 體力充足
        .Where(p => p.PitchCount == 0)       // 本場未上場
        .ToList();
    
    if (!availablePitchers.Any())
    {
        // 如果沒有理想選擇，選體力最好的
        availablePitchers = bullpen
            .OrderByDescending(p => p.CurrentStamina)
            .Take(1)
            .ToList();
    }
    
    // 根據局數選擇適當角色
    if (state.CurrentInning >= 9 && IsLeading(state))
    {
        // 終結者
        var closer = availablePitchers
            .FirstOrDefault(p => p.Role == (int)PitcherRole.Closer);
        if (closer != null) return closer;
    }
    else if (state.CurrentInning == 8)
    {
        // 佈局投手
        var setupMan = availablePitchers
            .FirstOrDefault(p => p.Role == (int)PitcherRole.SetupMan);
        if (setupMan != null) return setupMan;
    }
    
    // 選擇中繼投手（能力最強者）
    return availablePitchers
        .OrderByDescending(p => p.Control + p.Velocity)
        .FirstOrDefault();
}

private bool IsLeading(GameState state)
{
    return state.IsTopInning 
        ? state.HomeScore > state.AwayScore 
        : state.AwayScore > state.HomeScore;
}
```

### 3. 選擇優先序

```
1. 角色匹配度（終結者/佈局/中繼）
2. 體力狀況（優先選滿體力）
3. 出賽狀況（優先選未上場）
4. 能力值總和（Control + Velocity + Breaking）
5. 特定能力需求（領先選 Control 高，落後選 Velocity 高）
```

---

## 實作架構

### 1. PitcherManager 服務類別

```csharp
namespace simulator_console.Services;

public class PitcherManager
{
    private readonly Random _random = new();
    
    /// <summary>
    /// 初始化投手體力與狀態
    /// </summary>
    public void InitializePitcher(Player pitcher)
    {
        pitcher.CurrentStamina = 100.0;
        pitcher.PitchCount = 0;
        pitcher.ConsecutiveHitsAllowed = 0;
    }
    
    /// <summary>
    /// 更新投手體力（每投一球/每個打席）
    /// </summary>
    public void UpdateStamina(Player pitcher, AtBatResult result, int basesOccupied)
    {
        // 計算基礎消耗
        double baseStaminaCost = 100.0 / (pitcher.Stamina * 0.8);
        
        // 根據結果調整消耗
        double multiplier = result.ResultType switch
        {
            AtBatType.StrikeOut => 1.2,
            AtBatType.Walk => 1.5,
            AtBatType.HomeRun => 2.0,
            AtBatType.Double or AtBatType.Triple or AtBatType.Single => 1.3,
            _ => 1.0
        };
        
        double staminaCost = baseStaminaCost * multiplier;
        
        // 壘上有人額外消耗
        staminaCost += basesOccupied * 2.0;
        
        // 更新體力（不低於0）
        pitcher.CurrentStamina = Math.Max(0, pitcher.CurrentStamina - staminaCost);
        pitcher.PitchCount++;
        
        // 更新連續被安打
        if (result.ResultType == AtBatType.Single || 
            result.ResultType == AtBatType.Double || 
            result.ResultType == AtBatType.Triple || 
            result.ResultType == AtBatType.HomeRun)
        {
            pitcher.ConsecutiveHitsAllowed++;
        }
        else
        {
            pitcher.ConsecutiveHitsAllowed = 0;
        }
    }
    
    /// <summary>
    /// 檢查是否需要換投
    /// </summary>
    public PitcherChangeDecision ShouldChangePitcher(Player pitcher, GameState state)
    {
        // 實作如上述判定邏輯
    }
    
    /// <summary>
    /// 選擇後援投手
    /// </summary>
    public Player? SelectReliever(List<Player> bullpen, GameState state)
    {
        // 實作如上述選擇邏輯
    }
    
    /// <summary>
    /// 應用疲勞效果到投手能力
    /// </summary>
    public Player ApplyFatigueEffect(Player pitcher)
    {
        // 創建一個臨時副本，不修改原始數據
        var fatigued = new Player
        {
            PlayerId = pitcher.PlayerId,
            Name = pitcher.Name,
            TeamId = pitcher.TeamId,
            Type = pitcher.Type,
            Role = pitcher.Role,
            CurrentStamina = pitcher.CurrentStamina,
            PitchCount = pitcher.PitchCount,
            // 複製所有屬性...
        };
        
        // 應用疲勞效果
        if (pitcher.CurrentStamina >= 70)
        {
            // 無影響
            return pitcher;
        }
        else if (pitcher.CurrentStamina >= 50)
        {
            double factor = 0.85 + (pitcher.CurrentStamina - 50) / 100.0;
            fatigued.Control = (int)(pitcher.Control * factor);
            fatigued.Breaking = (int)(pitcher.Breaking * factor);
        }
        else if (pitcher.CurrentStamina >= 30)
        {
            double factor = 0.7 + (pitcher.CurrentStamina - 30) / 100.0;
            fatigued.Control = (int)(pitcher.Control * factor);
            fatigued.Velocity = (int)(pitcher.Velocity * 0.95);
            fatigued.Breaking = (int)(pitcher.Breaking * factor);
        }
        else
        {
            double factor = 0.5 + pitcher.CurrentStamina / 100.0;
            fatigued.Control = (int)(pitcher.Control * factor);
            fatigued.Velocity = (int)(pitcher.Velocity * 0.85);
            fatigued.Breaking = (int)(pitcher.Breaking * factor);
        }
        
        return fatigued;
    }
}
```

### 2. 修改 Simulator.cs

```csharp
// 在 SimulateAtBat 方法開始前應用疲勞
public AtBatResult SimulateAtBat(Player batter, Player pitcher, PitcherManager pitcherManager)
{
    // 應用疲勞效果（不修改原始對象）
    var fatiguedPitcher = pitcherManager.ApplyFatigueEffect(pitcher);
    
    // 使用疲勞後的能力進行對決
    var atBatResult = new AtBatResult { Hitter = batter, Pitcher = pitcher };
    
    if (CheckWalk(batter, fatiguedPitcher))
    {
        atBatResult.ResultType = AtBatType.Walk;
        return atBatResult;
    }
    
    double attack = 
        batter.Contact * 0.5 +
        batter.Power * 0.3 +
        batter.Vision * 0.2;
    
    double defense =
        fatiguedPitcher.Control * 0.3 +
        fatiguedPitcher.Velocity * 0.4 +
        fatiguedPitcher.Breaking * 0.3;
    
    // ... 其餘邏輯不變
}
```

### 3. 修改 GameSimulatorService.cs

```csharp
private readonly PitcherManager _pitcherManager;

public GameSimulatorSerivce(/* 現有參數 */)
{
    // ...
    _pitcherManager = new PitcherManager();
}

private void InitializeGame()
{
    // ... 現有初始化邏輯
    
    // 初始化投手
    _pitcherManager.InitializePitcher(_gameState.HomeTeamPitcher);
    _pitcherManager.InitializePitcher(_gameState.AwayTeamPitcher);
    
    // 初始化牛棚（從資料庫載入後援投手）
    _gameState.HomeTeamBullpen = GetBullpenPitchers(_gameState.HomeTeamRoster);
    _gameState.AwayTeamBullpen = GetBullpenPitchers(_gameState.AwayTeamRoster);
}

private List<Player> GetBullpenPitchers(List<Player> roster)
{
    return roster
        .Where(p => p.Type == 1) // 投手
        .Where(p => p.Role != (int)PitcherRole.Starter) // 非先發
        .ToList();
}

private void RunHalfInningSync(List<string> logs)
{
    // 半局開始前檢查是否需要換投
    CheckAndChangePitcher(logs);
    
    _gameState.ResetForNewHalfInning();
    
    while (_gameState.Outs < 3)
    {
        var currentBatter = GetCurrentBatter();
        var currentPitcher = GetCurrentPitcher();
        var basesOccupied = CountBasesOccupied();
        
        // 執行打席（傳入 pitcherManager）
        var result = _simulator.SimulateAtBat(currentBatter, currentPitcher, _pitcherManager);
        
        // 更新投手體力
        _pitcherManager.UpdateStamina(currentPitcher, result, basesOccupied);
        
        // 處理打席結果
        ProcessAtBatResult(result, logs);
        
        // 檢查是否需要緊急換投
        var decision = _pitcherManager.ShouldChangePitcher(currentPitcher, _gameState);
        if (decision.ShouldChange && decision.Priority <= 2)
        {
            ChangePitcher(logs, decision.Reason);
        }
        
        AdvanceBatter();
    }
}

private void CheckAndChangePitcher(List<string> logs)
{
    var currentPitcher = GetCurrentPitcher();
    var decision = _pitcherManager.ShouldChangePitcher(currentPitcher, _gameState);
    
    if (decision.ShouldChange)
    {
        ChangePitcher(logs, decision.Reason);
    }
}

private void ChangePitcher(List<string> logs, PitcherChangeReason reason)
{
    bool isHomeTeam = !_gameState.IsTopInning;
    var bullpen = isHomeTeam ? _gameState.HomeTeamBullpen : _gameState.AwayTeamBullpen;
    var oldPitcher = isHomeTeam ? _gameState.HomeTeamPitcher : _gameState.AwayTeamPitcher;
    
    var newPitcher = _pitcherManager.SelectReliever(bullpen, _gameState);
    
    if (newPitcher != null)
    {
        // 記錄換投
        var change = new PitcherChange
        {
            Inning = _gameState.CurrentInning,
            IsTopInning = _gameState.IsTopInning,
            OldPitcherName = oldPitcher.Name,
            NewPitcherName = newPitcher.Name,
            Reason = reason.ToString()
        };
        _gameState.PitcherChanges.Add(change);
        
        // 執行換投
        if (isHomeTeam)
            _gameState.HomeTeamPitcher = newPitcher;
        else
            _gameState.AwayTeamPitcher = newPitcher;
        
        // 初始化新投手
        _pitcherManager.InitializePitcher(newPitcher);
        
        // 從牛棚移除
        bullpen.Remove(newPitcher);
        
        // 記錄日誌
        string reasonText = reason switch
        {
            PitcherChangeReason.Fatigue => "體力耗盡",
            PitcherChangeReason.PitchCount => "投球數過多",
            PitcherChangeReason.RunsAllowed => "失分過多",
            PitcherChangeReason.Emergency => "連續被安打",
            PitcherChangeReason.InningLimit => "局數限制",
            _ => "戰術換投"
        };
        
        logs.Add($"🔄 換投：{newPitcher.Name} 上場（{reasonText}）");
    }
    else
    {
        logs.Add($"⚠️ 牛棚無可用投手，繼續使用 {oldPitcher.Name}");
    }
}

private int CountBasesOccupied()
{
    int count = 0;
    for (int i = 1; i <= 3; i++)
    {
        if (_gameState.Bases[i] != null) count++;
    }
    return count;
}
```

---

## 資料庫調整

### 1. Schema 修改

```sql
-- 新增 Role 欄位到 Players 表
ALTER TABLE Players ADD COLUMN Role INTEGER DEFAULT 0;
```

### 2. 更新種子資料

```sql
-- 設定投手角色
-- 先發投手
UPDATE Players SET Role = 0 WHERE Name LIKE '%Pitcher%' AND Type = 1;

-- 可以手動指定特定投手的角色
UPDATE Players SET Role = 3 WHERE Name = 'Home Closer';  -- 終結者
UPDATE Players SET Role = 2 WHERE Name = 'Setup Pitcher';  -- 佈局投手
UPDATE Players SET Role = 1 WHERE Name LIKE '%Reliever%';  -- 中繼投手
```

### 3. 新增後援投手種子資料

```sql
-- Home Team 牛棚
INSERT INTO Players (TeamId, Name, Type, Role, Stamina, Control, Velocity, Breaking) 
VALUES 
(2, 'Home Middle Reliever 1', 1, 1, 65, 75, 85, 70),
(2, 'Home Middle Reliever 2', 1, 1, 70, 70, 80, 75),
(2, 'Home Setup Man', 1, 2, 60, 85, 90, 80),
(2, 'Home Closer', 1, 3, 55, 90, 95, 85);

-- Away Team 牛棚
INSERT INTO Players (TeamId, Name, Type, Role, Stamina, Control, Velocity, Breaking) 
VALUES 
(1, 'Away Middle Reliever 1', 1, 1, 65, 72, 82, 72),
(1, 'Away Middle Reliever 2', 1, 1, 68, 68, 78, 78),
(1, 'Away Setup Man', 1, 2, 62, 82, 88, 82),
(1, 'Away Closer', 1, 3, 58, 88, 92, 88);
```

---

## 開發步驟

### Phase 1: 資料模型與基礎設施（Day 1）

- [x] **Task 1.1**: 更新 `Player.cs` 模型
  - 新增 `Role` 屬性
  - 新增 `CurrentStamina`, `PitchCount`, `ConsecutiveHitsAllowed` (NotMapped)

- [x] **Task 1.2**: 更新 `GameState.cs` 模型
  - 新增投手狀態追蹤屬性
  - 新增牛棚管理屬性
  - 新增失分追蹤屬性

- [x] **Task 1.3**: 建立枚舉類型
  - `PitcherRole` 枚舉
  - `PitcherChangeReason` 枚舉
  - `PitcherChange` 記錄類別

- [x] **Task 1.4**: 更新資料庫 Schema
  - 執行 ALTER TABLE 新增 Role 欄位
  - 更新種子資料

### Phase 2: 核心服務實作（Day 2）

- [x] **Task 2.1**: 建立 `PitcherManager` 服務
  - 實作 `InitializePitcher` 方法
  - 實作 `UpdateStamina` 方法
  - 實作 `ApplyFatigueEffect` 方法

- [x] **Task 2.2**: 實作換投判定邏輯
  - 實作 `ShouldChangePitcher` 方法
  - 定義優先級系統

- [x] **Task 2.3**: 實作後援投手選擇
  - 實作 `SelectReliever` 方法
  - 實作角色匹配邏輯

### Phase 3: 整合與測試（Day 3）

- [x] **Task 3.1**: 修改 `Simulator.cs`
  - 整合 `PitcherManager` 到打席模擬
  - 應用疲勞效果

- [x] **Task 3.2**: 修改 `GameSimulatorService.cs`
  - 初始化投手與牛棚
  - 實作換投流程
  - 整合體力更新

- [x] **Task 3.3**: 註冊服務
  - 在 `Program.cs` 註冊 `PitcherManager`

### Phase 4: 調整與優化（Day 4）

- [ ] **Task 4.1**: 平衡性調整
  - 測試體力消耗速率
  - 調整換投閾值
  - 微調能力下降幅度

- [ ] **Task 4.2**: 日誌與通知
  - 完善換投日誌訊息
  - SignalR 推送換投通知

- [ ] **Task 4.3**: 統計整合
  - 記錄投手投球局數
  - 更新 PitchingStats

---

## 測試情境

### 基本測試案例

```csharp
[Test Case 1] 投球數觸發換投
- 先發投手投 101 球
- 預期：自動換投
- 驗證：新投手體力 100%

[Test Case 2] 體力耗盡換投
- 投手體力降至 18%
- 預期：強制換投
- 驗證：換投原因為 Fatigue

[Test Case 3] 單局失分換投
- 單局失 3 分
- 預期：自動換投
- 驗證：換投原因為 RunsAllowed

[Test Case 4] 連續被安打換投
- 連續被打 3 支安打
- 預期：緊急換投
- 驗證：換投原因為 Emergency

[Test Case 5] 局數限制換投
- 先發投手投完 7 局
- 預期：自動換投佈局投手
- 驗證：新投手 Role = SetupMan

[Test Case 6] 終結者登場
- 第 9 局領先
- 預期：終結者上場
- 驗證：新投手 Role = Closer

[Test Case 7] 牛棚耗盡
- 所有後援投手已用完
- 預期：繼續使用當前投手
- 驗證：顯示警告訊息

[Test Case 8] 疲勞影響能力
- 投手體力 35%
- 預期：Control/Breaking 下降約 30%
- 驗證：對決失分率提高
```

### 整合測試

```csharp
[Integration Test 1] 完整比賽流程
1. 先發投手投 6 局
2. 中繼投手投 2 局
3. 終結者投 1 局
驗證：換投時機正確

[Integration Test 2] 體力消耗曲線
1. 記錄每球後體力
2. 繪製體力曲線圖
驗證：消耗速率合理

[Integration Test 3] 失分壓力測試
1. 模擬連續失分情境
2. 檢查換投反應
驗證：換投及時
```

---

## 未來擴充功能

### Phase 2 功能（v0.3）

- 🔮 **投手暖身系統**
  - 後援投手需要 1-2 個打席準備
  - 暖身中投手能力略降
  - 暖身完成後恢復正常

- 🔮 **投手休息日管理**（賽季模式）
  - 追蹤投手每場使用狀況
  - 自動排休（投滿局數後休 3-4 天）
  - 休息不足影響能力

- 🔮 **手動換投功能**
  - 前端提供換投按鈕
  - 教練可主動換投
  - 顯示牛棚投手列表與狀態

### Phase 3 功能（v0.4）

- 🔮 **投手受傷風險**
  - 疲勞過度增加受傷機率
  - 投球數超標有受傷風險
  - 受傷後進入傷兵名單

- 🔮 **教練換投策略**
  - 保守型：較早換投
  - 激進型：讓先發投更多局
  - 平衡型：標準換投時機

- 🔮 **投手熱度系統**
  - 狀態好時能力提升
  - 狀態差時能力下降
  - 影響換投決策

### Phase 4 功能（v0.5）

- 🔮 **進階數據追蹤**
  - 投手用球分佈（快速球/變化球）
  - 投球熱區分析
  - 對特定打者效果

- 🔮 **AI 學習換投決策**
  - 根據歷史數據優化換投時機
  - 學習玩家換投習慣
  - 提供換投建議

---

## 附錄

### A. 參數調整指南

```csharp
// 體力消耗速率（可調整）
BaseStaminaCost = 100.0 / (Stamina * 0.8);  // 調整 0.8 可改變整體消耗速度

// 換投閾值（可調整）
FatigueThreshold = 20;      // 體力低於此值強制換投
PitchCountLimit = 100;      // 投球數上限
RunsAllowedThreshold = 3;   // 單局失分上限
ConsecutiveHitsLimit = 3;   // 連續被安打上限

// 疲勞影響係數（可調整）
LightFatigueFactor = 0.85;  // 輕度疲勞時能力保留 85%
ModerateFatigueFactor = 0.7;  // 中度疲勞時能力保留 70%
SevereFatigueFactor = 0.5;    // 嚴重疲勞時能力保留 50%
```

### B. 故障排除

**問題 1：投手體力下降過快**
- 檢查 BaseStaminaCost 計算公式
- 調整 Stamina 屬性值
- 減少事件消耗倍率

**問題 2：換投太頻繁**
- 提高換投閾值
- 降低優先級權重
- 調整半局檢查邏輯

**問題 3：牛棚投手不足**
- 增加後援投手數量
- 降低後援投手體力要求
- 實作投手重複使用機制

---

## 版本歷程

- **v1.0** (2025-11-24)
  - ✅ 初始規格文件
  - ✅ 完整系統設計
  - ✅ 實作指南

---

**文件維護者：** AI Assistant  
**最後更新：** 2025-11-24  
**狀態：** 📝 規格完成，準備實作
