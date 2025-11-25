# 《實況野球風格賽季制棒球遊戲》開發進度追蹤

📅 最後更新：2025-11-24

## 專案概覽

本文件追蹤 SRS.md 規劃的所有功能項目與實際開發完成狀況。

---

## 1. Season Mode（賽季模式）

### 功能需求狀態

| 需求編號 | 需求描述 | 狀態 | 完成度 | 備註 |
|---------|---------|------|--------|------|
| F1.1 | 系統能生成完整賽季賽程 | ✅ 已完成 | 100% | `SeasonService.GenerateSchedule()` 實作雙循環賽程 |
| F1.2 | 玩家可選擇自動模擬/親自操作比賽 | ⚠️ 部分完成 | 50% | 已實作自動模擬，手動操作待開發 |
| F1.3 | 系統記錄球隊排名 | ❌ 未完成 | 0% | 缺少排名計算與顯示功能 |
| F1.4 | 系統在賽季結束後頒發獎項 | ❌ 未完成 | 0% | 未實作獎項系統 |

**已實作檔案：**
- `Models/Season.cs` - 賽季資料模型
- `Models/Schedule.cs` - 賽程資料模型
- `Services/SeasonService.cs` - 賽季服務
- `Controllers/SeasonController.cs` - 賽季 API

**缺少功能：**
- 排名計算邏輯
- 勝敗統計
- 年度獎項評選系統
- 季後賽系統

---

## 2. Team Management（球隊管理）

### 功能需求狀態

| 需求編號 | 需求描述 | 狀態 | 完成度 | 備註 |
|---------|---------|------|--------|------|
| F2.1 | 查看球員列表與屬性 | ✅ 已完成 | 100% | `TeamController.GetRoster()` API |
| F2.2 | 編輯打序、守備、輪值 | ❌ 未完成 | 0% | 缺少陣容編輯功能 |
| F2.3 | 進行交易（未來功能） | ❌ 未開始 | 0% | 規劃中功能 |
| F2.4 | 設定球隊策略 | ❌ 未完成 | 0% | 未實作策略系統 |

**已實作檔案：**
- `Models/Team.cs` - 球隊基本模型
- `Models/TeamEntity.cs` - 球隊資料庫實體
- `Controllers/TeamController.cs` - 球隊 API（查詢功能）

**缺少功能：**
- 打序編輯器
- 守備位置指派
- 投手輪值排程
- 球隊戰術設定

---

## 3. Player System（球員系統）

### 功能需求狀態

| 需求編號 | 需求描述 | 狀態 | 完成度 | 備註 |
|---------|---------|------|--------|------|
| F3.1 | 球員屬性影響比賽結果 | ✅ 已完成 | 100% | `Simulator.SimulateAtBat()` 使用屬性計算 |
| F3.2 | 球員可透過訓練提升能力 | ❌ 未完成 | 0% | 缺少訓練系統 |
| F3.3 | 球員可退化（年齡） | ❌ 未完成 | 0% | 無年齡增長與退化機制 |
| F3.4 | 球員可能受傷 | ❌ 未完成 | 0% | 無受傷系統 |
| F3.5 | 球員具有潛力曲線 | ❌ 未完成 | 0% | 無潛力與成長系統 |

### 球員屬性實作狀況

**打者屬性（與 SRS 對照）：**

| SRS 屬性 | 實作屬性 | 狀態 | 備註 |
|---------|---------|------|------|
| Contact | Contact | ✅ 已修正 | 已從 Contract 修正 |
| Power | Power | ✅ | |
| Vision | Vision | ✅ 已修正 | 已從 Discipline 修正 |
| Speed | Speed | ✅ | |
| Fielding | Fielding | ✅ | |
| Arm | Arm | ✅ | |
| Reaction | - | ❌ 缺少 | 未實作 |

**投手屬性（與 SRS 對照）：**

| SRS 屬性 | 實作屬性 | 狀態 | 備註 |
|---------|---------|------|------|
| Velocity | Velocity | ✅ | |
| Control | Control | ✅ | |
| Breaking | Breaking | ✅ 已修正 | 已從 Movement 修正 |
| Stamina | Stamina | ✅ | |

**其他屬性：**

| SRS 屬性 | 實作狀況 | 狀態 |
|---------|---------|------|
| 年齡 (Age) | ❌ 未實作 | |
| 潛力 (Potential) | ❌ 未實作 | |
| 健康 (Health) | ❌ 未實作 | |
| 疲勞 (Fatigue) | ❌ 未實作 | |
| 特殊能力 | ❌ 未實作 | |

**已實作檔案：**
- `Models/Player.cs` - 球員資料模型
- `Services/PlayerPackage/PlayerSerivce.cs` - 球員服務
- `Services/PlayerPackage/PlayerType.cs` - 球員類型定義

**缺少功能：**
- 訓練系統
- 年齡與退化系統
- 受傷機制
- 潛力成長曲線
- 特殊能力系統
- Reaction 屬性（反應力）

---

## 4. Match Engine（比賽模擬系統）

### 功能需求狀態

| 需求編號 | 需求描述 | 狀態 | 完成度 | 備註 |
|---------|---------|------|--------|------|
| F4.1 | 比賽模擬能產生完整 Box Score | ⚠️ 部分完成 | 60% | 有基本比分，缺完整統計 |
| F4.2 | AI 根據球員屬性選擇戰術 | ❌ 未完成 | 0% | 無 AI 戰術決策 |
| F4.3 | 投手疲勞影響控球與失分 | ❌ 未完成 | 0% | 投手疲勞未實作 |
| F4.4 | 模擬結果需可重現（RNG Seed） | ❌ 未完成 | 0% | 未使用固定種子 |

### 比賽邏輯實作狀況

**已實作機制：**
- ✅ 投打對決計算（`Simulator.SimulateAtBat()`）
- ✅ 擊球品質判定（WeakGroundBall, GroundBall, LineDrive, FlyBall, DeepFlyBall, Bomb）✨ 新增
- ✅ 守備系統（`DefenseManager.ProcessDefense()`）✨ 新增
- ✅ 守備失誤機制（根據守備能力和難度判定）✨ 新增
- ✅ 擊球結果：三振、出局、安打、二壘打、三壘打、全壘打
- ✅ 保送判定（`CheckWalk()`）
- ✅ 跑壘系統（`GameSimulatorSerivce` 處理壘包前進）
- ✅ 計分系統
- ✅ 9 局賽制
- ✅ 投手疲勞系統（`PitcherManager`）✨ 新增
- ✅ 自動換投機制 ✨ 新增

**缺少機制：**
- ✅ 守備成功率判定（已完成 DefenseManager）
- ✅ 守備失誤系統（已整合）
- ❌ 投手體力消耗與換投（已實作投手疲勞系統，待完善）
- ❌ AI 戰術決策（短打、盜壘、保送策略）
- ❌ 延長賽
- ❌ 詳細的 Box Score（打擊率、長打率等）
- ❌ 可重現的隨機種子系統

**已實作檔案：**
- `Services/Simulator/Simulator.cs` - 核心模擬邏輯
- `Services/Simulator/GameSimulatorSerivce.cs` - 比賽流程控制
- `Services/DefenseManager.cs` - 守備系統 ✨ 新增
- `Services/PitcherManager.cs` - 投手疲勞管理 ✨ 新增
- `Models/GameState.cs` - 比賽狀態管理
- `Models/AtBatResult.cs` - 打席結果（含擊球品質）✨ 更新
- `Controllers/GameController.cs` - 比賽 API

**缺少功能：**
- 完整 Box Score 統計
- AI 教練決策
- 隨機種子管理
- 延長賽機制

---

## 5. Statistics System（數據系統）

### 功能需求狀態

| 需求編號 | 需求描述 | 狀態 | 完成度 | 備註 |
|---------|---------|------|--------|------|
| F5.1 | 球員紀錄：AVG, HR, RBI, ERA, WHIP | ✅ 已完成 | 100% | 已建立統計表與計算邏輯 |
| F5.2 | 進階數據：OPS, BABIP（可選） | ⚠️ 部分完成 | 50% | OPS 已實作，BABIP 未實作 |
| F5.3 | 球隊紀錄：勝敗、排名 | ❌ 未完成 | 0% | 未實作 |
| F5.4 | 系統自動運算年度獎項 | ❌ 未完成 | 0% | 未實作 |

**已實作檔案：**
- `Models/GameRecord.cs` - 比賽紀錄
- `Models/GameLog.cs` - 比賽日誌
- `Models/HittingStats.cs` - 打者統計模型 ✨ 新增
- `Models/PitchingStats.cs` - 投手統計模型 ✨ 新增
- `Services/StatisticsService.cs` - 統計服務 ✨ 新增
- `Controllers/StatisticsController.cs` - 統計 API ✨ 新增

**已完成功能：** ✨
- ✅ 打者統計表（AVG, OBP, SLG, OPS, HR, RBI）
- ✅ 投手統計表（ERA, WHIP, K/9, BB/9, W-L）
- ✅ 統計自動計算
- ✅ 排行榜查詢 API
- ✅ 球員賽季統計查詢

**缺少功能：**
- 球隊勝敗統計與排名
- BABIP 進階數據
- 獎項評選算法
- 統計整合至比賽模擬（需在 GameSimulatorService 中呼叫）

---

## 6. Save System（儲存系統）

### 功能需求狀態

| 需求編號 | 需求描述 | 狀態 | 完成度 | 備註 |
|---------|---------|------|--------|------|
| F6.1 | 可存檔（JSON / SQLite） | ✅ 已完成 | 100% | 使用 SQLite + EF Core |
| F6.2 | 可讀檔 | ✅ 已完成 | 100% | 透過 API 讀取 |
| F6.3 | 多存檔槽 | ❌ 未完成 | 0% | 目前單一資料庫 |
| F6.4 | 自動保存（比賽後） | ✅ 已完成 | 100% | `SaveGameResult()` |

**已實作檔案：**
- `Data/GameDbContext.cs` - 資料庫上下文
- `RebuildSqls/schema.sql` - 資料庫結構
- `RebuildSqls/seed.sql` - 種子資料

**缺少功能：**
- 多存檔槽管理
- 存檔備份
- 存檔損壞檢測

---

## 7. UI / UX Modules（使用者介面）

### 功能需求狀態

| 模組 | 狀態 | 完成度 | 備註 |
|------|------|--------|------|
| Main Menu | ❌ 未開始 | 0% | 需前端實作 |
| Team Management Screens | ❌ 未開始 | 0% | 需前端實作 |
| Season Management | ❌ 未開始 | 0% | 需前端實作 |
| Match UI | ⚠️ 部分 | 20% | 有 SignalR 即時推送 |
| Box Score 頁面 | ❌ 未開始 | 0% | 需前端實作 |

**已實作：**
- `Hubs/GameHub.cs` - SignalR 即時通訊
- REST API 端點（後端完成）

**缺少：**
- 完整前端 UI（Blazor WebAssembly）
- 使用者互動介面

---

## 8. System Architecture（系統架構）

### 技術堆疊實作狀況

| 技術 | SRS 規劃 | 實際使用 | 狀態 |
|------|---------|---------|------|
| 後端框架 | ASP.NET Core | ASP.NET Core (.NET 9) | ✅ |
| 資料庫 | SQLite / PostgreSQL | SQLite + EF Core | ✅ |
| 前端 | Blazor WebAssembly | React | ⚠️ 規劃中 |
| 即時通訊 | - | SignalR | ✅ 已實作 |
| UI 框架 | Tailwind / Radzen | Tailwind CSS (推薦) | ⚠️ 規劃中 |

### 前端技術選擇說明

**React + SignalR 的優勢：**
- ✅ **生態系統豐富** - npm 套件多，社群支援強
- ✅ **開發體驗佳** - Hot Reload、開發工具成熟
- ✅ **SignalR 客戶端** - `@microsoft/signalr` 官方支援完整
- ✅ **彈性高** - 可自由選擇狀態管理（Redux、Zustand）
- ✅ **效能優異** - Virtual DOM 與現代優化技術
- ✅ **UI 框架選擇多** - Material-UI、Ant Design、Chakra UI、Tailwind

**與 Blazor WebAssembly 比較：**

| 特性 | React | Blazor WebAssembly |
|------|-------|-------------------|
| 語言 | JavaScript/TypeScript | C# |
| 生態系統 | 極大 | 較小 |
| 學習曲線 | 平緩 | 對 .NET 開發者友好 |
| 效能 | 優秀 | 優秀（初始載入較慢） |
| 與後端整合 | REST API + SignalR | 原生 .NET |
| 開發工具 | 成熟完整 | 持續改進中 |

**推薦技術棧：**
```
前端：React 18+ + TypeScript
狀態管理：Zustand 或 Redux Toolkit
UI 框架：Tailwind CSS + shadcn/ui 或 Material-UI
即時通訊：@microsoft/signalr
打包工具：Vite
```

**SignalR 整合範例：**
```typescript
// SignalR 連接設定
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://localhost:7xxx/gameHub")
  .withAutomaticReconnect()
  .build();

// 監聽比賽更新
connection.on("ReceiveGameUpdate", (update) => {
  console.log("Game Update:", update);
  // 更新 React state
});

await connection.start();
```

---

## 9. Database Design（資料庫設計）

### 資料表實作狀況

| SRS 規劃資料表 | 實作狀況 | 狀態 | 備註 |
|--------------|---------|------|------|
| Players | ✅ 已完成 | 100% | 屬性命名已修正，缺少欄位（Age, Potential, Health, Fatigue） |
| Teams | ✅ 已完成 | 100% | |
| Season | ✅ 已完成 | 100% | |
| Games | ✅ 已完成 | 100% | 名稱為 GameRecords |
| Stats_Hitting | ✅ 已完成 | 100% | ✨ 已新增完整打者統計表 |
| Stats_Pitching | ✅ 已完成 | 100% | ✨ 已新增完整投手統計表 |
| Schedules | ✅ 已完成 | 100% | SRS 未規劃但已實作 |

---

## 10. Non-Functional Requirements（非功能性需求）

### 性能需求

| 需求 | 目標 | 實作狀況 | 狀態 |
|------|------|---------|------|
| 模擬一場比賽時間 | < 1 秒 | 未測試 | ⚠️ |
| UI 操作反應時間 | < 100ms | 無 UI | ❌ |

### 可靠性

| 需求 | 實作狀況 | 狀態 |
|------|---------|------|
| 自動備份存檔 | ❌ 未實作 | ❌ |
| 防止存檔損壞 | ❌ 未實作 | ❌ |

---

## 11. Future Enhancements（未來功能）

所有未來功能均為 ❌ 未開始：

- ❌ 線上 Multiplayer（PvP）
- ❌ 卡片化球員（抽卡系統）
- ❌ 球隊 Logo 自訂器
- ❌ 球員臉部生成
- ❌ 更多實況野球風技能
- ❌ 球場模擬、氣象系統
- ❌ 球探與新人選秀系統

---

## 總體完成度統計

### 功能模組完成度

| 模組 | 完成度 | 狀態 |
|------|--------|------|
| Season Mode | 25% | 🟡 進行中 |
| Team Management | 25% | 🟡 進行中 |
| Player System | 40% | 🟡 進行中 |
| Match Engine | 85% | 🟢 接近完成 |
| Statistics System | 70% | 🟢 接近完成 |
| Save System | 75% | 🟢 接近完成 |
| UI / UX | 5% | 🔴 未開始 |
| Database | 90% | 🟢 接近完成 |

### 整體專案完成度

```
████████████████░░░░ 65%
```

**已完成功能（65%）：**
- ✅ 基本比賽模擬引擎
- ✅ 守備系統與失誤機制 ✨ 新增
- ✅ 投手疲勞與換投系統 ✨ 新增
- ✅ 擊球品質判定系統 ✨ 新增
- ✅ 資料庫架構（含統計表）
- ✅ 賽季賽程生成
- ✅ 球員與球隊資料模型（屬性已修正）
- ✅ REST API 基礎
- ✅ SignalR 即時通訊
- ✅ 統計系統（打者/投手統計、排行榜）

**進行中功能（20%）：**
- ⚠️ 統計系統與比賽整合
- ⚠️ 球員成長與訓練系統
- ⚠️ 陣容編輯功能
- ⚠️ AI 戰術決策（短打、盜壘等）

**未開始功能（15%）：**
- ❌ 前端 UI（React）
- ❌ 球隊排名系統
- ❌ 年度獎項系統
- ❌ 延長賽機制

---

## 優先開發建議

### 🔥 高優先級（核心遊戲體驗）

1. ~~**完善比賽模擬系統**~~ ✅ **大部分完成**
   - ✅ 投手疲勞與換投
   - ✅ 守備失誤機制
   - ⚠️ AI 戰術決策（待實作）

2. **球員統計系統**
   - ✅ 個人數據計算（AVG, ERA 等）
   - ⚠️ 即時統計更新（待整合）
   - ✅ 歷史紀錄

3. **陣容管理功能**
   - 打序編輯
   - 守備位置指派
   - 投手輪值

### 🟡 中優先級（遊戲深度）

4. **球員成長系統**
   - 訓練機制
   - 年齡與退化
   - 潛力系統

5. **排名與獎項**
   - 球隊勝敗統計
   - 排行榜
   - 年度獎項

6. **前端 UI（Blazor）**
   - 主選單
   - 賽程介面
   - 比賽觀戰畫面

### 🟢 低優先級（擴充功能）

7. 特殊能力系統
8. 受傷機制
9. 多存檔槽
10. 未來功能（抽卡、多人等）

---

## 技術債務與問題

### ⚠️ 需要修正的問題

1. ~~**屬性命名不一致**~~ ✅ **已修正**
   - ~~`Contract` 應為 `Contact`~~ ✅ 已修正
   - ~~`Discipline` 應為 `Vision`~~ ✅ 已修正
   - ~~`Movement` 應為 `Breaking`~~ ✅ 已修正

2. **缺少重要屬性**
   - 球員：Age, Potential, Health, Fatigue
   - 投手：當前體力狀態（Fatigue）
   - 守備：Reaction（反應力）

3. **比賽邏輯不完整**
   - 無守備失誤
   - 無投手疲勞系統
   - 無延長賽

4. ~~**資料庫結構**~~ ✅ **已修正**
   - ~~缺少 Stats_Hitting 和 Stats_Pitching 表~~ ✅ 已新增

5. **統計系統整合** ⚠️ **待完成**
   - 需在 GameSimulatorService 中整合 StatisticsService
   - 比賽結束後自動更新球員統計

---

## 版本歷程

- **v0.3** (目前版本) - 2025-11-25 ✨
  - ✅ 實作守備系統（DefenseManager）
  - ✅ 擊球品質判定（BallQuality 枚舉）
  - ✅ 守備失誤機制
  - ✅ 投手疲勞系統（PitcherManager）
  - ✅ 自動換投決策
  - ✅ 調整投打對決機率（提高擊球率，減少三振）
  - ✅ 整合守備系統至比賽模擬流程

- **v0.2** - 2025-11-24 ✨
  - ✅ 修正球員屬性命名（Contact, Vision, Breaking）
  - ✅ 新增統計系統（HittingStats, PitchingStats）
  - ✅ 新增統計服務與 API
  - ✅ 資料庫結構完善
  - ⚠️ 統計系統待整合至比賽模擬

- **v0.1** (2025-11-24 前)
  - 基本比賽模擬
  - 資料庫架構
  - REST API
  - SignalR 整合

---

**備註：** 本文件應隨開發進度定期更新。
