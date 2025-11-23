-- 甲球隊
INSERT INTO teams (name, city)
VALUES
    ('Dragons', 'Tokyo'),
    ('Sharks', 'Osaka');

-- 球員資料
INSERT INTO players (name, team_id, position, stamina, control, velocity, movement, contact, power)
VALUES
    ('Yu Darvish', 1, 'P', 85, 78, 90, 82, 25, 20);
INSERT INTO players (name, team_id, position, contact, power, discipline, speed, baserunning)
VALUES
    ('Taro Suzuki', 1, 'OF', 70, 65, 60, 75, 70),
    ('Kenji Mori', 1, '1B', 60, 80, 55, 40, 45),
    ('Shun Takahashi', 1, 'SS', 75, 55, 70, 85, 80);

INSERT INTO players (name, team_id, position, stamina, control, velocity, movement, contact, power)
VALUES
    ('Kodai Senga', 2, 'P', 88, 75, 92, 85, 20, 15);

INSERT INTO players (name, team_id, position, contact, power, discipline, speed, baserunning)
VALUES
    ('Riku Yamamoto', 2, 'CF', 72, 68, 65, 80, 75),
    ('Hiro Tanaka', 2, '3B', 65, 75, 58, 45, 50),
    ('Sho Murata', 2, 'C', 58, 70, 55, 30, 40);