using simulator_console.Data;
using simulator_console.Models;

namespace simulator_console.Services;

public class SeasonService
{
    private readonly GameDbContext _context;

    public SeasonService(GameDbContext context)
    {
        _context = context;
    }

    public Season StartNewSeason(int year)
    {
        var season = new Season { Year = year };
        _context.Seasons.Add(season);
        _context.SaveChanges();

        GenerateSchedule(season.Id);
        return season;
    }

    public List<Schedule> GenerateSchedule(string seasonId)
    {
        var schedules = GenerateScheduleList(seasonId);
        _context.Schedules.AddRange(schedules);
        _context.SaveChanges();
        return _context.Schedules.Where(s => s.SeasonId == seasonId).ToList();
    }

    public List<Schedule> GenerateScheduleList(string seasonId)
    {
        var teams = _context.Teams.ToList();
        if (teams.Count < 2) return new List<Schedule>();

        // Simple Round Robin: Each team plays every other team once
        // For a real season, we'd want more games.
        // Let's do a double round robin (Home and Away)
        
        int day = 1;
        for (int i = 0; i < teams.Count; i++)
        {
            for (int j = i + 1; j < teams.Count; j++)
            {
                // Game 1: i vs j
                _context.Schedules.Add(new Schedule
                {
                    SeasonId = seasonId,
                    Day = day,
                    HomeTeamId = teams[i].Id,
                    AwayTeamId = teams[j].Id
                });
                
                // Game 2: j vs i (next day or same day?) Let's spread them out
                _context.Schedules.Add(new Schedule
                {
                    SeasonId = seasonId,
                    Day = day + 1,
                    HomeTeamId = teams[j].Id,
                    AwayTeamId = teams[i].Id
                });
                
                day += 2; 
            }
        }
        _context.SaveChanges();
        return _context.Schedules.Where(s => s.SeasonId == seasonId).ToList();
    }
}
