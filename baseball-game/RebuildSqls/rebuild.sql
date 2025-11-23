-- for postgresql

-- TEAM DDL
CREATE TABLE teams (
                       team_id SERIAL PRIMARY KEY,
                       name VARCHAR(100) NOT NULL ,
                       city VARCHAR(100),
                       created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE players (
                         player_id SERIAL PRIMARY KEY,
                         name VARCHAR(100) NOT NULL,
                         team_id INT REFERENCES teams(team_id) ON DELETE SET NULL,

    -- 打者能力值
                         contact SMALLINT NOT NULL DEFAULT 30,  -- 揮棒準度 
                         power SMALLINT NOT NULL DEFAULT 30, -- 長打能力
                         discipline SMALLINT NOT NULL DEFAULT 30, -- 選球能力

    -- 跑壘能力
                         speed SMALLINT NOT NULL DEFAULT 30, -- 速度
                         baserunning SMALLINT NOT NULL DEFAULT 30, -- 跑壘判斷

    -- 防守能力
                         fielding SMALLINT NOT NULL DEFAULT  30, -- 接球
                         arm SMALLINT NOT NULL DEFAULT 30, -- 臂力
                         position VARCHAR(5) NOT NULL DEFAULT 'OF', -- 守備位置

    -- 頭球能力
                         stamina SMALLINT NOT NULL DEFAULT 20, -- 體力
                         control SMALLINT NOT NULL DEFAULT 20, -- 控球
                         velocity SMALLINT NOT NULL DEFAULT 20, -- 球速
                         movement SMALLINT NOT NULL DEFAULT 20,     -- 球路變化 (0-100)

                         experience SMALLINT NOT NULL DEFAULT 1, -- 經驗/等級

    -- 系統欄位
                         create_at TIMESTAMP NOT NULL DEFAULT NOW(),
                         update_at TIMESTAMP NOT NULL DEFAULT NOW()
);

