-- Insert Teams
INSERT INTO Teams (Id, Name, Abbreviation, Color) VALUES (1, 'Away Team', 'AWY', '#FF0000');
INSERT INTO Teams (Id, Name, Abbreviation, Color) VALUES (2, 'Home Team', 'HOM', '#0000FF');

-- Insert Players
-- Away Team (TeamId = 1)
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (1, 'Away Batter 1', 0, 50, 60, 70);
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (1, 'Away Batter 2', 0, 50, 65, 60);
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (1, 'Away Batter 3', 0, 50, 70, 50);
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (1, 'Away Batter 4', 0, 50, 80, 40);
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (1, 'Away Batter 5', 0, 50, 75, 45);
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (1, 'Away Batter 6', 0, 50, 60, 60);
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (1, 'Away Batter 7', 0, 50, 55, 65);
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (1, 'Away Batter 8', 0, 50, 50, 70);
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (1, 'Away Batter 9', 0, 50, 45, 75);

-- Home Team (TeamId = 2)
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (2, 'Home Batter 1', 0, 50, 60, 70);
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (2, 'Home Batter 2', 0, 50, 65, 60);
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (2, 'Home Batter 3', 0, 50, 70, 50);
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (2, 'Home Batter 4', 0, 50, 80, 40);
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (2, 'Home Batter 5', 0, 50, 75, 45);
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (2, 'Home Batter 6', 0, 50, 60, 60);
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (2, 'Home Batter 7', 0, 50, 55, 65);
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (2, 'Home Batter 8', 0, 50, 50, 70);
INSERT INTO Players (TeamId, Name, Type, Contract, Power, Discipline) VALUES (2, 'Home Batter 9', 0, 50, 45, 75);

-- Pitchers
INSERT INTO Players (TeamId, Name, Type, Stamina, Control, Movement, Velocity) VALUES (1, 'Away Pitcher', 1, 80, 80, 80, 95);
INSERT INTO Players (TeamId, Name, Type, Stamina, Control, Movement, Velocity) VALUES (2, 'Home Pitcher', 1, 85, 85, 85, 98);
