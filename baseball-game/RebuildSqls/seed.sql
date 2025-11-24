-- Insert Teams
INSERT INTO Teams (Id, Name, Abbreviation, Color) VALUES (1, 'Away Team', 'AWY', '#FF0000');
INSERT INTO Teams (Id, Name, Abbreviation, Color) VALUES (2, 'Home Team', 'HOM', '#0000FF');

-- Insert Players
-- Away Team (TeamId = 1)
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (1, 'Away Batter 1', 0, 50, 60, 70);
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (1, 'Away Batter 2', 0, 50, 65, 60);
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (1, 'Away Batter 3', 0, 50, 70, 50);
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (1, 'Away Batter 4', 0, 50, 80, 40);
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (1, 'Away Batter 5', 0, 50, 75, 45);
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (1, 'Away Batter 6', 0, 50, 60, 60);
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (1, 'Away Batter 7', 0, 50, 55, 65);
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (1, 'Away Batter 8', 0, 50, 50, 70);
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (1, 'Away Batter 9', 0, 50, 45, 75);

-- Home Team (TeamId = 2)
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (2, 'Home Batter 1', 0, 50, 60, 70);
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (2, 'Home Batter 2', 0, 50, 65, 60);
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (2, 'Home Batter 3', 0, 50, 70, 50);
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (2, 'Home Batter 4', 0, 50, 80, 40);
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (2, 'Home Batter 5', 0, 50, 75, 45);
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (2, 'Home Batter 6', 0, 50, 60, 60);
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (2, 'Home Batter 7', 0, 50, 55, 65);
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (2, 'Home Batter 8', 0, 50, 50, 70);
INSERT INTO Players (TeamId, Name, Type, Contact, Power, Vision) VALUES (2, 'Home Batter 9', 0, 50, 45, 75);

-- Pitchers
-- Away Team Pitchers (TeamId = 1)
INSERT INTO Players (TeamId, Name, Type, Role, Stamina, Control, Breaking, Velocity) VALUES (1, 'Away Starter', 1, 0, 80, 80, 80, 95);
INSERT INTO Players (TeamId, Name, Type, Role, Stamina, Control, Breaking, Velocity) VALUES (1, 'Away Middle Reliever 1', 1, 1, 60, 70, 75, 85);
INSERT INTO Players (TeamId, Name, Type, Role, Stamina, Control, Breaking, Velocity) VALUES (1, 'Away Middle Reliever 2', 1, 1, 65, 72, 73, 83);
INSERT INTO Players (TeamId, Name, Type, Role, Stamina, Control, Breaking, Velocity) VALUES (1, 'Away Setup Man', 1, 2, 55, 85, 80, 90);
INSERT INTO Players (TeamId, Name, Type, Role, Stamina, Control, Breaking, Velocity) VALUES (1, 'Away Closer', 1, 3, 50, 90, 85, 95);

-- Home Team Pitchers (TeamId = 2)
INSERT INTO Players (TeamId, Name, Type, Role, Stamina, Control, Breaking, Velocity) VALUES (2, 'Home Starter', 1, 0, 85, 85, 85, 98);
INSERT INTO Players (TeamId, Name, Type, Role, Stamina, Control, Breaking, Velocity) VALUES (2, 'Home Middle Reliever 1', 1, 1, 62, 73, 77, 87);
INSERT INTO Players (TeamId, Name, Type, Role, Stamina, Control, Breaking, Velocity) VALUES (2, 'Home Middle Reliever 2', 1, 1, 68, 75, 75, 85);
INSERT INTO Players (TeamId, Name, Type, Role, Stamina, Control, Breaking, Velocity) VALUES (2, 'Home Setup Man', 1, 2, 58, 88, 82, 92);
INSERT INTO Players (TeamId, Name, Type, Role, Stamina, Control, Breaking, Velocity) VALUES (2, 'Home Closer', 1, 3, 52, 92, 88, 98);
