-- Clear existing data
DELETE FROM Players;

-- Away Team (TeamId = 1)
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (1, 'Away Batter 1', 0, 80, 70, 50);
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (1, 'Away Batter 2', 0, 75, 65, 50);
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (1, 'Away Batter 3', 0, 90, 80, 50);
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (1, 'Away Batter 4', 0, 85, 75, 50);
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (1, 'Away Batter 5', 0, 70, 60, 50);
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (1, 'Away Batter 6', 0, 65, 55, 50);
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (1, 'Away Batter 7', 0, 60, 50, 50);
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (1, 'Away Batter 8', 0, 55, 45, 50);
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (1, 'Away Batter 9', 0, 50, 40, 50);

-- Home Team (TeamId = 2)
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (2, 'Home Batter 1', 0, 82, 72, 50);
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (2, 'Home Batter 2', 0, 77, 67, 50);
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (2, 'Home Batter 3', 0, 92, 82, 50);
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (2, 'Home Batter 4', 0, 87, 77, 50);
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (2, 'Home Batter 5', 0, 72, 62, 50);
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (2, 'Home Batter 6', 0, 67, 57, 50);
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (2, 'Home Batter 7', 0, 62, 52, 50);
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (2, 'Home Batter 8', 0, 57, 47, 50);
INSERT INTO Players (TeamId, Name, Type, Power, Discipline, Contract) VALUES (2, 'Home Batter 9', 0, 52, 42, 50);

-- Pitchers
INSERT INTO Players (TeamId, Name, Type, Stamina, Control, Movement, Velocity) VALUES (1, 'Away Pitcher', 1, 80, 80, 80, 95);
INSERT INTO Players (TeamId, Name, Type, Stamina, Control, Movement, Velocity) VALUES (2, 'Home Pitcher', 1, 85, 85, 85, 98);
