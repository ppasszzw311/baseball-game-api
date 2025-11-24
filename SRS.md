🎮 《實況野球風格賽季制棒球遊戲》— SRS 文件
1. Introduction（簡介）
1.1 Purpose（目的）

本文件用於定義《實況野球風格賽季制棒球遊戲》的所有功能、架構、邏輯與技術規格。
開發目標為建立一款具有賽季制、球隊經營、球員養成、比賽模擬的棒球遊戲。
此 SRS 提供 AI 代理、開發者或團隊完整參考。

1.2 Scope（範圍）

遊戲包括：

賽季系統（Season Mode）

球隊管理（Team Management）

球員屬性與養成（Player Attributes & Development）

比賽模擬系統（Match Engine）

數據紀錄（Stats）

儲存系統（Save / Load）

UI 介面流程（UI / UX）

平台：PC / Web（Blazor WebAssembly）
遊戲類型：2D 卡通 Q 版，《實況野球》風格

2. Overall Description（整體描述）
2.1 Game Concept

以《實況野球（Power Pros）》為主要參考

畫風：2D Q 版、卡通、簡化球場場景

遊戲包括手動操作與純模擬兩模式（未來可加入手動操作）

2.2 User Characteristics

棒球遊戲玩家

輕度玩家／數據派玩家

成就導向：喜歡養成、經營球隊

支持離線單機

2.3 Assumptions and Dependencies

使用者具備基本棒球規則認知

遊戲運行於現代瀏覽器（Chrome、Edge、Safari）

如新增多人對戰需伺服器端支援

3. System Features（系統功能）
3.1 Season Mode（賽季模式）
Description

玩家以單一球隊參加整季賽事、季後賽，持續累積多賽季紀錄。

Functional Requirements

F1.1 系統能生成完整賽季賽程

F1.2 玩家可選擇自動模擬 / 親自操作比賽

F1.3 系統記錄球隊排名

F1.4 系統在賽季結束後比照 MLB 頒發獎項

3.2 Team Management（球隊管理）
Description

玩家可管理球員陣容、排陣、訓練與球隊策略。

Functional Requirements

F2.1 查看球員列表與屬性

F2.2 編輯打序、守備、輪值

F2.3 進行交易（未來功能）

F2.4 設定球隊策略（保守/激進等）

3.3 Player System（球員系統）
Description

每位球員具有屬性、成長、狀態、特殊能力。

Attributes

打者屬性：

Contact

Power

Vision

Speed

Fielding

Arm

Reaction

投手屬性：

Velocity

Control

Breaking

Stamina

其他：

年齡

潛力（Potential）

健康（Health）

疲勞（Fatigue）

特殊能力（如：左殺、怪力、 clutch）

Functional Requirements

F3.1 球員屬性影響比賽結果

F3.2 球員可透過訓練提升能力

F3.3 球員可退化（年齡）

F3.4 球員可能受傷，受傷期間無法上場

F3.5 球員具有潛力曲線決定最大成長值

3.4 Match Engine（比賽模擬系統）
Description

比賽以簡化方式呈現，投打對決由機率演算法決定。

Key Mechanics

投手 vs 打者判定

擊球結果模型：滾地、飛球、全壘打、三振

跑壘機率模型

守備成功率判定

投手體力消耗

AI 戰術（是否換投、是否短打）

Functional Requirements

F4.1 比賽模擬能產生完整 Box Score

F4.2 AI 根據球員屬性選擇戰術

F4.3 投手疲勞影響控球與失分

F4.4 模擬結果需可重現（根據 RNG Seed）

3.5 Statistics System（數據系統）
Description

記錄球員賽季、隊史數據，提供玩家分析。

Functional Requirements

F5.1 球員紀錄：AVG, HR, RBI, ERA, WHIP

F5.2 進階數據：OPS, BABIP（可選）

F5.3 球隊紀錄：勝敗、排名

F5.4 系統自動運算年度獎項

3.6 Save System（儲存系統）
Functional Requirements

F6.1 可存檔（JSON / SQLite）

F6.2 可讀檔

F6.3 多存檔槽

F6.4 自動保存（比賽後）

4. External Interface Requirements（外部介面需求）
4.1 UI / UX Modules

以下介面須被建立：

Main Menu

Start Game

Load Game

Settings

Team Management Screens

Roster（球員列表）

Player Details

Lineup Editor（打序編輯）

Rotation Editor（先發投手）

Training Page

Season Management

Schedule（賽程）

Standings（排名）

League Leaders

Awards Page

Match UI

打擊 / 投球圖示

即時數據

動畫（可簡化）

比賽結果頁（Box Score）

5. System Architecture（系統架構）
5.1 技術架構（Tech Stack）
Frontend

Blazor WebAssembly

Canvas / SVG（比賽演出）

Tailwind / Radzen（UI 框架可選）

Backend（如需）

ASP.NET Core Web API

EF Core / Dapper

PostgreSQL / SQLite（初期建議 SQLite）

AI Modules（可選）

AI 生成賽程

AI 模擬比賽（計算引擎）

AI 球探與球員生成

6. Database Design（資料庫設計）
6.1 Tables（SQLite / PostgreSQL）
Players
Column	Type	Description
player_id	int	PK
name	text	球員姓名
age	int	年齡
potential	int	潛力
role	text	投手/打者
contact	int	
power	int	
vision	int	
speed	int	
fielding	int	
arm	int	
reaction	int	
velocity	int	
control	int	
breaking	int	
stamina	int	
health	int	
fatigue	int	
Teams
Column	Type	Description
team_id	int	PK
name	text	球隊
logo	blob / url	logo
Season

season_id

year

current_day

Games

game_id

season_id

home_team

away_team

result

boxscore JSON

Stats_Hitting / Stats_Pitching

紀錄球員賽季數據。

7. Game Logic Specifications（遊戲邏輯規格）
7.1 比賽邏輯（核心）

（供 AI 直接照邏輯寫 code）

投打對決公式（示例）
HitChance = (Contact + Vision*0.5) - (Pitcher.Control * 0.7)
PowerEffect = Power * 1.2 - Pitcher.Velocity * 0.5

安打機率分類

5%：全壘打（受 Power 影響）

15%：長打（二壘安打）

20%：平飛安打

25%：滾地安打

10%：界外球

25%：出局
（AI 可自行調整）

投手疲勞模型

每投一球疲勞值上升，當 Fatigue > 70 時發生失投機率提升。

8. Non-Functional Requirements（非功能性需求）
8.1 Performance

模擬一場比賽須在 1 秒內 完成（純模擬模式）

UI 操作須 <100ms 反應

8.2 Reliability

自動備份存檔

防止存檔損壞

8.3 Portability

Web 版本可跨平台（Windows / macOS / iOS / Android）

9. Future Enhancements（未來功能）

線上 Multiplayer（PvP）

卡片化球員（抽卡系統）

球隊 Logo 自訂器

球員臉部生成

更多實況野球風技能

球場模擬、氣象系統

球探與新人選秀系統