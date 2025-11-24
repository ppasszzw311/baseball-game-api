namespace simulator_console.Models;

public class GameResult
{
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public string Winner { get; set; } = string.Empty;
    public List<string> GameLogs { get; set; } = new();
}
