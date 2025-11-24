CREATE TABLE IF NOT EXISTS Players (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    TeamId INTEGER NOT NULL,
    Name TEXT NOT NULL,
    Type INTEGER NOT NULL, -- 0: Hitter, 1: Pitcher
    Role INTEGER DEFAULT 0, -- 0: Starter, 1: MiddleReliever, 2: SetupMan, 3: Closer
    -- Hitting attributes
    Contact INTEGER DEFAULT 30,
    Power INTEGER DEFAULT 15,
    Vision INTEGER DEFAULT 20,
    -- Pitching attributes
    Stamina INTEGER DEFAULT 30,
    Control INTEGER DEFAULT 20,
    Velocity INTEGER DEFAULT 15,
    Breaking INTEGER DEFAULT 14,
    -- Base running
    Speed INTEGER DEFAULT 0,
    BaseRunning INTEGER DEFAULT 0,
    -- Fielding
    Fielding INTEGER DEFAULT 0,
    Arm INTEGER DEFAULT 0,
    Position INTEGER DEFAULT 0,
    -- Experience
    Experience INTEGER DEFAULT 0
);

CREATE TABLE IF NOT EXISTS GameRecords (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    HomeScore INTEGER NOT NULL,
    AwayScore INTEGER NOT NULL,
    Winner TEXT NOT NULL,
    PlayedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS GameLogs (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    GameId INTEGER NOT NULL,
    LogMessage TEXT,
    FOREIGN KEY (GameId) REFERENCES GameRecords(Id)
);

CREATE TABLE IF NOT EXISTS Seasons (
    Id TEXT PRIMARY KEY,
    Year INTEGER NOT NULL,
    CurrentDay INTEGER NOT NULL,
    CreatedAt TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Teams (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Abbreviation TEXT NOT NULL,
    Color TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Schedules (
    Id TEXT PRIMARY KEY,
    SeasonId TEXT NOT NULL,
    Day INTEGER NOT NULL,
    HomeTeamId INTEGER NOT NULL,
    AwayTeamId INTEGER NOT NULL,
    IsPlayed INTEGER NOT NULL,
    GameRecordId INTEGER,
    FOREIGN KEY (SeasonId) REFERENCES Seasons(Id),
    FOREIGN KEY (HomeTeamId) REFERENCES Teams(Id),
    FOREIGN KEY (AwayTeamId) REFERENCES Teams(Id)
);

CREATE TABLE IF NOT EXISTS Stats_Hitting (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PlayerId INTEGER NOT NULL,
    SeasonId TEXT NOT NULL,
    -- Plate Appearances
    PlateAppearances INTEGER DEFAULT 0,  -- 打席數
    AtBats INTEGER DEFAULT 0,            -- 打數 (不含保送、觸身球等)
    -- Hits
    Hits INTEGER DEFAULT 0,              -- 安打
    Singles INTEGER DEFAULT 0,           -- 一壘安打
    Doubles INTEGER DEFAULT 0,           -- 二壘安打
    Triples INTEGER DEFAULT 0,           -- 三壘安打
    HomeRuns INTEGER DEFAULT 0,          -- 全壘打
    -- Runs and RBIs
    Runs INTEGER DEFAULT 0,              -- 得分
    RBI INTEGER DEFAULT 0,               -- 打點
    -- Walks and Strikeouts
    Walks INTEGER DEFAULT 0,             -- 保送
    Strikeouts INTEGER DEFAULT 0,        -- 三振
    -- Other
    StolenBases INTEGER DEFAULT 0,       -- 盜壘成功
    CaughtStealing INTEGER DEFAULT 0,    -- 盜壘失敗
    -- Calculated Stats (可在應用層計算，或用觸發器更新)
    BattingAverage REAL DEFAULT 0.0,     -- 打擊率 (Hits / AtBats)
    OnBasePercentage REAL DEFAULT 0.0,   -- 上壘率 (H+BB+HBP) / (AB+BB+HBP+SF)
    SluggingPercentage REAL DEFAULT 0.0, -- 長打率 (TB / AB)
    OPS REAL DEFAULT 0.0,                -- 上壘率+長打率
    FOREIGN KEY (PlayerId) REFERENCES Players(Id),
    FOREIGN KEY (SeasonId) REFERENCES Seasons(Id)
);

CREATE TABLE IF NOT EXISTS Stats_Pitching (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PlayerId INTEGER NOT NULL,
    SeasonId TEXT NOT NULL,
    -- Games and Innings
    GamesPlayed INTEGER DEFAULT 0,       -- 出賽數
    GamesStarted INTEGER DEFAULT 0,      -- 先發場次
    InningsPitched REAL DEFAULT 0.0,     -- 投球局數
    -- Outs and Batters Faced
    BattersFaced INTEGER DEFAULT 0,      -- 面對打者數
    -- Hits and Runs Allowed
    HitsAllowed INTEGER DEFAULT 0,       -- 被安打
    RunsAllowed INTEGER DEFAULT 0,       -- 失分
    EarnedRuns INTEGER DEFAULT 0,        -- 自責分
    HomeRunsAllowed INTEGER DEFAULT 0,   -- 被全壘打
    -- Walks and Strikeouts
    WalksAllowed INTEGER DEFAULT 0,      -- 保送
    Strikeouts INTEGER DEFAULT 0,        -- 三振
    -- Wins and Losses
    Wins INTEGER DEFAULT 0,              -- 勝場
    Losses INTEGER DEFAULT 0,            -- 敗場
    Saves INTEGER DEFAULT 0,             -- 救援成功
    -- Calculated Stats
    ERA REAL DEFAULT 0.0,                -- 防禦率 (ER * 9 / IP)
    WHIP REAL DEFAULT 0.0,               -- (BB + H) / IP
    StrikeoutsPer9 REAL DEFAULT 0.0,     -- K/9
    WalksPer9 REAL DEFAULT 0.0,          -- BB/9
    FOREIGN KEY (PlayerId) REFERENCES Players(Id),
    FOREIGN KEY (SeasonId) REFERENCES Seasons(Id)
);
